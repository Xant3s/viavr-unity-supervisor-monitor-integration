using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Signaling;
using UnityEngine;

public class ServerDiscovery : MonoBehaviour {
    private volatile string serverAddress;
    private volatile string receivedMessage;
    private volatile bool serverDiscovered;
    private volatile string sanitizedServerAddress;
    private bool serverSetup;
    private volatile bool connectionAlive;
    private volatile Socket connectionSocket;
    private Thread serverNotifier;

    public void BreakConnection() {
        serverNotifier.Abort();
        connectionSocket.Close();
    }

    private void Start() {
        var daemonThread = new Thread(ScanPortInSystem) {
            IsBackground = true
        };
        daemonThread.Start();
    }

    private void Update() {
        if(serverSetup || !serverDiscovered) return;
        var sanitizedWsAddress = receivedMessage.Split(' ').ToList().Last();
        sanitizedServerAddress = serverAddress.Split(':')[0];
        ISignaling signaling = new WebSocketSignaling($"ws://{sanitizedWsAddress}", 5.0f, SynchronizationContext.Current);
        SignalingHandlerBase handlerBase = GetComponent<Broadcast>();
        GetComponent<RenderStreaming>().Run(true, signaling, new []{handlerBase});
        serverSetup = true;
        serverNotifier = new Thread(SendKeepAliveSignal);
        serverNotifier.Start();
    }

    private void OnDestroy() {
        BreakConnection();
        GetComponent<RenderStreaming>().Stop();
    }

    private void ScanPortInSystem() {
        Socket sock = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint iep = new IPEndPoint(IPAddress.Any, 41234);
        sock.Bind(iep);
        EndPoint ep = iep;
        Debug.Log("Waiting for streaming server");
        byte[] data = new byte[1024];
        int receivedDate = sock.ReceiveFrom(data, ref ep);
        string stringData = Encoding.ASCII.GetString(data, 0, receivedDate);
        Debug.Log($"received: {stringData} from: {ep}");
        sock.Close();
        serverAddress = ep.ToString();
        receivedMessage = stringData;
        serverDiscovered = true;
    }

    private void SendKeepAliveSignal() {
        /*connectionSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
        connectionSocket.Bind(iep);*/
        IPEndPoint iep = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 31234);
        var udpClient = new UdpClient();
        while(true){
            byte[] sendBuffer = Encoding.ASCII.GetBytes("Still sharing");
            udpClient.Send(sendBuffer, sendBuffer.Length, iep);
            /*Debug.Log($"Sending keep alive message to {iep.Address} on Port: {iep.Port}");
            connectionSocket.SendTo(sendBuffer, iep);
            Thread.Sleep(5000);*/
        }
    }
}
