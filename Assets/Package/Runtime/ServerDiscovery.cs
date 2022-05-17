using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Package.Runtime;
using UnityEngine;

public class ServerDiscovery : MonoBehaviour {
    private volatile FormattedIpAddress supervisorAddress;
    private volatile EndPoint ep;
    private volatile string receivedMessage;
    private volatile bool serverDiscovered;
    private volatile string sanitizedServerAddress;
    private bool serverSetup;
    private volatile bool connectionAlive;
    private volatile Socket supervisorSocket;
    
    private Thread serverNotifier;
    private Thread serverAwaiter;
    private Thread portScanThread;
    private Thread timeoutThread;

    private List<ServerTransmission> transmissions;

    private class FormattedIpAddress {
        public readonly IPAddress ipAddress;
        public readonly int port;

        public FormattedIpAddress(string address, int port) {
            ipAddress = IPAddress.Parse(address);
            this.port = port;
        }

        public static FormattedIpAddress ParseToAddress(string totalAddress) {
            string[] addressParts = totalAddress.Split(':');
            return new FormattedIpAddress(addressParts[0], int.Parse(addressParts[1]));
        }

        public override string ToString() {
            return $"{ipAddress}:{port}";
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
        BreakConnection();
        foreach(var dataTransmission in transmissions) {
            dataTransmission.StopTransmission();
        }
    }

    private void Start() {
        StartPortScanningThread();
    }

    private void StartPortScanningThread() {
        portScanThread?.Abort();
        portScanThread = new Thread(ScanPortInSystem) {
            IsBackground = true
        };
        portScanThread.Start();
    }

    private void Update() {
        if(serverSetup || !serverDiscovered) return;
        transmissions.Add(new WebStreamingTransmission());
        foreach(var serverTransmission in transmissions) {
            serverTransmission.StartTransmission(receivedMessage,transform);
        }
        serverSetup = true;
        serverNotifier = new Thread(SendKeepAliveSignal);
        serverAwaiter = new Thread(AwaitSupervisorAliveSignal);
        serverNotifier.Start();
        serverAwaiter.Start();
    }

    private void OnDestroy() {
        BreakConnection();
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
        receivedMessage = stringData;
        serverDiscovered = true;
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
        serverSetup = false;
        serverDiscovered = false;
        supervisorSocket.Close();
        StartPortScanningThread();
    }
}
