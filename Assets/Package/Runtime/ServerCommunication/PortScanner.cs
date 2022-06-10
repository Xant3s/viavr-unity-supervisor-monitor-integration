using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class PortScanner {
        private const string ConnectionMessage = "Looking for Client";
        
        private ConnectionInfo currentConnectionInfo;
        private Thread portScanThread;
        private bool connectToServer;
        private volatile FormattedIpAddress supervisorAddress;
        private readonly List<(ServerDiscovery.Transmission, FormattedIpAddress)> requestedTransmissions = new();
        
        public void StartScanner(Socket scannedPort, EndPoint endPoint) {
            portScanThread?.Abort();
            portScanThread = new Thread(() => ScanPortInSystem(scannedPort, endPoint)) {
                IsBackground = true
            };
            portScanThread.Start();
        }
        
        public void StopScanner() => portScanThread?.Abort();

        public void SetConnected() => connectToServer = false;

        public bool FoundServer(out FormattedIpAddress ipAddress, out ConnectionInfo newConnectionInfo) {
            ipAddress = supervisorAddress;
            newConnectionInfo = currentConnectionInfo;
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
                if (!messageData.Contains(ConnectionMessage)) return;
            } while (!AcknowledgeConnectionWithId(messageData));
            supervisorAddress = FormattedIpAddress.ParseToAddress(endPoint.ToString());
            connectToServer = true;
        }
        
        private bool AcknowledgeConnectionWithId(string receivedMessage)
        {
            var requestedServices = receivedMessage.Replace(ConnectionMessage, "");
            currentConnectionInfo = JsonUtility.FromJson<ConnectionInfo>(requestedServices);
            currentConnectionInfo.OnAfterDeserialize();
            if (currentConnectionInfo.ID == null) return false;
            Debug.Log($"Server Id is: {currentConnectionInfo.ID}");
            return true;
        }

    }
}