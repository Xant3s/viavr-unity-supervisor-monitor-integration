using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace de.jmu.ge.viavr.webstreaming {
    public class SupervisorDiscovery {
        public event EventHandler<IPEndPoint> OnSupervisorFound;
        private const string connectionMessage = "Supervisor monitor looking for client";
        private const int port = 41234;
        private readonly Timer timer = new();
        private Socket socket;
        private EndPoint endPoint;


        public void Start() {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(endPoint);
            timer.Interval = 500;
            timer.Elapsed += ListenForSupervisor;
            timer.Start();
        }

        public void Stop() {
            timer.Stop();
            socket?.Close();
        }

        private void ListenForSupervisor(object sender, ElapsedEventArgs e) => ListenForSupervisor();

        private void ListenForSupervisor() {
            var data = new byte[1024];
            var receivedData = socket.ReceiveFrom(data, ref endPoint);
            var messageData = Encoding.ASCII.GetString(data, 0, receivedData);
            if(!messageData.Contains(connectionMessage)) return;
            OnSupervisorFound?.Invoke(this, (IPEndPoint) endPoint);
        }
        
        ~SupervisorDiscovery() => Stop();
    }
}