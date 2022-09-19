using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace de.jmu.ge.viavr.webstreaming {
    public class SupervisorDiscovery {
        public event EventHandler<IPEndPoint> OnSupervisorFound;
        private const string connectionMessage = "Supervisor monitor looking for client";
        private const int port = 41234;
        private Socket supervisorSocket;
        private Thread listenThread;


        public void Start() {
            supervisorSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endPoint = new IPEndPoint(IPAddress.Any, port);
            supervisorSocket.Bind(endPoint);
            listenThread = new Thread(() => ListenForSupervisor(supervisorSocket, endPoint)) {
                IsBackground = true
            };
            listenThread.Start();
        }

        public void Stop() {
            listenThread?.Abort();
            supervisorSocket?.Close();
        }

        private void ListenForSupervisor(Socket socket, EndPoint endPoint) {
            string messageData;
            do {
                var data = new byte[1024];
                var receivedData = socket.ReceiveFrom(data, ref endPoint);
                messageData = Encoding.ASCII.GetString(data, 0, receivedData);
            } while (!messageData.Contains(connectionMessage));
            OnSupervisorFound?.Invoke(this, (IPEndPoint) endPoint);
        }
    }
}