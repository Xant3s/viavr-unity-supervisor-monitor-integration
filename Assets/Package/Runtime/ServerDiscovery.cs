using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Package.Runtime;
using UnityEngine;

public class ServerDiscovery : MonoBehaviour
{
    private const string ConnectionMessage = "Looking for Client;";
    
    private volatile FormattedIpAddress supervisorAddress;
    private EndPoint ep;
    private bool connectToServer;
    private volatile Socket supervisorSocket;
    
    private Thread serverNotifier;
    private Thread serverAwaiter;
    private volatile bool timedOut;
    private Thread portScanThread;
    private volatile bool interruptedTimeOut;
    private Thread timeoutThread;

    private readonly List<ServerTransmission> transmissions = new();
    private readonly List<(Transmission, FormattedIpAddress)> requestedTransmissions = new();

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
        timedOut = false;
        serverNotifier = new Thread(SendKeepAliveSignal);
        serverAwaiter = new Thread(AwaitSupervisorAliveSignal);
        serverNotifier.Start();
        serverAwaiter.Start();
    }

    private void OnDestroy() {
        SendDisconnectMessage();
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
        
        if (!stringData.Contains(ConnectionMessage)) return;
        var requestedServices = stringData.Replace(ConnectionMessage, "");
        IdentifyRequestedTransmissions(requestedServices);
        supervisorAddress = FormattedIpAddress.ParseToAddress(ep.ToString());
        connectToServer = true;
    }

    private void SendKeepAliveSignal() {
        IPEndPoint iep = new IPEndPoint(supervisorAddress.ipAddress, supervisorAddress.port);
        var udpClient = new UdpClient();
        while(!timedOut) {
            Debug.Log($"Sending keep alive message to {supervisorAddress}");
            byte[] sendBuffer = Encoding.ASCII.GetBytes("Still sharing");
            udpClient.Send(sendBuffer, sendBuffer.Length, iep);
            Thread.Sleep(5000);
        }
    }
    
    private void AwaitSupervisorAliveSignal() {
        while(!timedOut) {
            Debug.Log("Waiting for streaming server");
            byte[] data = new byte[1024];
            interruptedTimeOut = true;
            timeoutThread?.Interrupt();
            timeoutThread = new Thread(TimeOutTracker);
            interruptedTimeOut = false;
            timeoutThread.Start();
            bool correctMessage = false;
            while(!correctMessage) {
                int receivedDate = supervisorSocket.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, receivedDate);
                if(stringData.Equals("Supervisor Monitor alive")) correctMessage = true;
                else if(stringData.Equals("Disconnecting")) {
                    Debug.Log("Closing connection");
                    serverNotifier?.Abort();
                    portScanThread?.Abort();
                    timeoutThread?.Abort();
                    StopTransmission();
                    supervisorSocket?.Close();
                    StartPortScanningThread();
                    return;
                }
            }
            interruptedTimeOut = true;
            timeoutThread?.Interrupt();
            interruptedTimeOut = false;
            Debug.Log($"Received keep alive signal from: {ep}");
        }
    }

    private void TimeOutTracker() {
        Thread.Sleep(15000);
        
        if(interruptedTimeOut) return;
        
        Debug.Log("Closing connection");
        timedOut = true;
        serverAwaiter?.Interrupt();
        serverNotifier?.Interrupt();
        StopTransmission();
        supervisorSocket.Close();
        StartPortScanningThread();
    }

    private void IdentifyRequestedTransmissions(string receivedMessage) {
        string[] formattedMessages = receivedMessage.Split(',');
        foreach(var message in formattedMessages) {
            string[] formattedMessage = message.Split(':');
            if (!Enum.TryParse(formattedMessage[0], out Transmission transmissionType)) return;
            requestedTransmissions.Add((transmissionType, FormattedIpAddress.ParseToAddress(formattedMessage[1])));
        }
    }

    private void SendDisconnectMessage() {
        IPEndPoint iep = new IPEndPoint(supervisorAddress.ipAddress, supervisorAddress.port);
        var udpClient = new UdpClient();
        Debug.Log($"Sending disconnect message to {supervisorAddress}");
        byte[] sendBuffer = Encoding.ASCII.GetBytes("Disconnecting");
        udpClient.Send(sendBuffer, sendBuffer.Length, iep);
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
