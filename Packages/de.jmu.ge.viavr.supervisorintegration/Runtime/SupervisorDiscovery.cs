using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class SupervisorDiscovery {
        private const string connectionMessage = "Supervisor monitor looking for client";
        private const int port = 41234;
        private readonly WaitForRequest<string> waitForRequest;
        private readonly Socket socket;
        private EndPoint endPoint;

        public IPAddress Address { get; private set; }

        
        public SupervisorDiscovery(int frequency = 500, int timeout = -1) {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(endPoint);
            waitForRequest = new WaitForRequest<string>(ListenForSupervisor, Predicate, frequency, timeout);
        }

        public async Task SupervisorFound() => await waitForRequest.WaitUntil();

        private async Task<string> ListenForSupervisor() {
            var data = new byte[1024];
            var receivedData = socket.ReceiveFrom(data, ref endPoint);
            Address = ((IPEndPoint) endPoint).Address;
            var messageData = Encoding.ASCII.GetString(data, 0, receivedData);
            return await Task.FromResult(messageData);
        }
    
        private bool Predicate(string messageData) => messageData.Contains(connectionMessage);
        
        ~SupervisorDiscovery() => socket.Close();
    }
}