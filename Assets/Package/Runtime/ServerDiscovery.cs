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
    private bool serverSetup;
    
    void Start() {
        var portMessage = new Thread(ScanPortInSystem) {
            IsBackground = true
        };
        portMessage.Start();
    }

    private void Update() {
        if(!serverSetup && serverDiscovered) {
            string sanitizedWsAddress = receivedMessage.Split(' ').ToList().Last();
            string sanitizedAddress = serverAddress.Split(':')[0];
            ISignaling signaling = new WebSocketSignaling($"ws://{sanitizedWsAddress}", 5.0f, SynchronizationContext.Current);
            SignalingHandlerBase handlerBase = GetComponent<Broadcast>();
            GetComponent<RenderStreaming>().Run(true, signaling, new []{handlerBase});
            serverSetup = true;
        }
    }

    private void OnDestroy() {
        GetComponent<RenderStreaming>().Stop();
    }

    /*private async Task AsyncSearchForServer() {
        Thread portMessage = new Thread(ScanPortInSystem);
        portMessage.Start();
        portMessage.Join();
    }*/

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
}
