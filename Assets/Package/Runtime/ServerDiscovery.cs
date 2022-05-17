using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Package.Runtime;
using UnityEngine;

public class ServerDiscovery : MonoBehaviour {
    private volatile FormattedIpAddress supervisorAddress;
    private EndPoint ep;
    private bool connectToServer;
    private volatile Socket supervisorSocket;
    
    private Thread serverNotifier;
    private Thread serverAwaiter;
    private Thread portScanThread;
    private Thread timeoutThread;

    private readonly List<ServerTransmission> transmissions = new();
    private List<(Transmission, FormattedIpAddress)> requestedTransmissions = new();

    private enum Transmission {
        WebStreaming
    }

    private void Start() {
        StartPortScanningThread();
    }

    private void Update() {
        if(!connectToServer) return;
        foreach(var serverTransmission in requestedTransmissions) {
            switch(serverTransmission.Item1) {
                case Transmission.WebStreaming:
                    var webStreamer = new WebStreamingTransmission();
                    transmissions.Add(webStreamer);
                    webStreamer.StartTransmission(serverTransmission.Item2, transform);
                    break;
            }
        }
        connectToServer = false;
        serverNotifier = new Thread(SendKeepAliveSignal);
        serverAwaiter = new Thread(AwaitSupervisorAliveSignal);
        serverNotifier.Start();
        serverAwaiter.Start();
    }

    private void OnDestroy() {
        BreakConnection();
        StopTransmission();
    }

    private void StartPortScanningThread() {
        portScanThread?.Abort();
        portScanThread = new Thread(ScanPortInSystem) {
            IsBackground = true
        };
        portScanThread.Start();
    }

    private void ScanPortInSystem() {
        supervisorSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint iep = new IPEndPoint(IPAddress.Any, 41234);
        supervisorSocket.Bind(iep);
        ep = iep;
        Debug.Log("Waiting for streaming server");
        byte[] data = new byte[1024];
        int receivedDate = supervisorSocket.ReceiveFrom(data, ref ep);
        string stringData = Encoding.ASCII.GetString(data, 0, receivedDate);
        Debug.Log($"received: {stringData} from: {ep}");
        supervisorAddress = FormattedIpAddress.ParseToAddress(ep.ToString());
        IdentifyRequestedTransmissions(stringData);
        connectToServer = true;
    }

    private void SendKeepAliveSignal() {
        IPEndPoint iep = new IPEndPoint(supervisorAddress.ipAddress, supervisorAddress.port);
        var udpClient = new UdpClient();
        while(true) {
            Debug.Log($"Sending keep alive message to {supervisorAddress}");
            byte[] sendBuffer = Encoding.ASCII.GetBytes("Still sharing");
            udpClient.Send(sendBuffer, sendBuffer.Length, iep);
            Thread.Sleep(5000);
        }
    }
    
    private void AwaitSupervisorAliveSignal() {
        while(true) {
            Debug.Log("Waiting for streaming server");
            byte[] data = new byte[1024];
            timeoutThread?.Abort();
            timeoutThread = new Thread(TimeOutTracker);
            timeoutThread.Start();
            bool correctMessage = false;
            while(!correctMessage) {
                int receivedDate = supervisorSocket.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, receivedDate);
                if(stringData.Equals("Supervisor Monitor alive")) correctMessage = true;
            }
            timeoutThread?.Abort();
            Debug.Log($"Received keep alive signal from: {ep}");
        }
    }

    private void TimeOutTracker() {
        Thread.Sleep(15000);
        Debug.Log("Closing connection");
        serverAwaiter?.Abort();
        serverNotifier?.Abort();
        StopTransmission();
        supervisorSocket.Close();
        StartPortScanningThread();
    }

    private void IdentifyRequestedTransmissions(string receivedMessage) {
        string[] formattedMessages = receivedMessage.Split(',');
        foreach(var message in formattedMessages) {
            string[] formattedMessage = message.Split(':');
            Transmission type;
            switch(formattedMessage[0]) {
                case "WebStreaming":
                    requestedTransmissions.Add((Transmission.WebStreaming, FormattedIpAddress.ParseToAddress(formattedMessage[1])));
                    break;
            }
        }
    }

    private void BreakConnection() {
        serverNotifier?.Abort();
        serverAwaiter?.Abort();
        portScanThread?.Abort();
        timeoutThread?.Abort();
        supervisorSocket?.Close();
    }
    
    private void StopTransmission() {
        foreach(var dataTransmission in transmissions) {
            dataTransmission.StopTransmission();
        }
        transmissions.Clear();
    }
}
