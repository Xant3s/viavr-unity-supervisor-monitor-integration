using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class PortScanner {
        private const string ConnectionMessage = "Looking for Client";
        
        private Thread portScanThread;
        private bool connectToServer;
        private volatile FormattedIpAddress supervisorAddress;
        
        public void StartScanner(Socket scannedPort, EndPoint endPoint) {
            portScanThread?.Abort();
            portScanThread = new Thread(() => ScanPortInSystem(scannedPort, endPoint)) {
                IsBackground = true
            };
            portScanThread.Start();
        }
        
        public void StopScanner() => portScanThread?.Abort();

        public void SetConnected() => connectToServer = false;

        public bool FoundServer(out FormattedIpAddress ipAddress) {
            ipAddress = supervisorAddress;
            return connectToServer;
        }
        
        private void ScanPortInSystem(Socket scannedPort, EndPoint endPoint) {
            Debug.Log("Waiting for streaming server");
            string messageData;
            do
            {
                byte[] data = new byte[1024];
                int receivedDate = scannedPort.ReceiveFrom(data, ref endPoint);
                messageData = Encoding.ASCII.GetString(data, 0, receivedDate);
                Debug.Log($"received: {messageData} from: {endPoint}");
            } while (!messageData.Contains(ConnectionMessage));
            supervisorAddress = FormattedIpAddress.ParseToAddress(endPoint.ToString());
            connectToServer = true;
        }
    }
}