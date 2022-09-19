using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace de.jmu.ge.viavr.webstreaming {
    public class SupervisorDiscovery : MonoBehaviour {
        public UnityEvent<IPEndPoint> onSupervisorFound;
        private const string connectionMessage = "Supervisor monitor looking for client";
        private Socket supervisorSocket;
        private Thread listenThread;
        private IPEndPoint supervisorEndPoint;
        private bool first;
        

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
            supervisorEndPoint = (IPEndPoint) endPoint;
            Debug.Log($"Supervisor found at {supervisorEndPoint.Address}:{supervisorEndPoint.Port}");
            // onSupervisorFound?.Invoke(supervisorEndPoint);
        }

        private void Update() {
            if(supervisorEndPoint != null && !first) {
                onSupervisorFound?.Invoke(supervisorEndPoint);
                first = true;
            }
        }

        public void Stop() {
            listenThread?.Abort();
            supervisorSocket?.Close();
        }

        private void OnDestroy() => Stop();
    }
}