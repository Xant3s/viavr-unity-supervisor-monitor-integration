using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Package.Runtime.Communication;
using UnityEngine;

namespace de.jmu.ge.viavr.webstreaming {
    public class SupervisorDiscovery : MonoBehaviour {
        private const string connectionMessage = "Supervisor monitor looking for client";
        private Socket supervisorSocket;
        private Thread listenThread;


        private void Start() {
            supervisorSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endPoint = new IPEndPoint(IPAddress.Any, 41234);
            supervisorSocket.Bind(endPoint);
            listenThread?.Abort();
            listenThread = new Thread(() => ListenForSupervisor(supervisorSocket, endPoint)) {
                IsBackground = true
            };
            listenThread.Start();
        }

        private void ListenForSupervisor(Socket scannedPort, EndPoint endPoint) {
            Debug.Log("Waiting for supervisor");
            string messageData;
            do {
                byte[] data = new byte[1024];
                int receivedDate = scannedPort.ReceiveFrom(data, ref endPoint);
                messageData = Encoding.ASCII.GetString(data, 0, receivedDate);
                Debug.Log($"received: {messageData} from {endPoint}");
            } while (!messageData.Contains(connectionMessage));
            var supervisorAddress = FormattedIpAddress.ParseToAddress(endPoint.ToString());
            Debug.Log($"Supervisor found at {supervisorAddress}");
        }

        public void Stop() {
            listenThread?.Abort();
            supervisorSocket?.Close();
        }

        private void OnDestroy() => Stop();
    }
}