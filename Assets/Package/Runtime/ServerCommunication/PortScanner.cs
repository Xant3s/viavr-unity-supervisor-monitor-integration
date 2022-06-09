using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class PortScanner {
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

        public bool FoundServer(out FormattedIpAddress ipAddress, out List<(ServerDiscovery.Transmission, FormattedIpAddress)> requestedTransmissions) {
            ipAddress = supervisorAddress;
            requestedTransmissions = this.requestedTransmissions;
            return connectToServer;
        }
        
        private void ScanPortInSystem(Socket scannedPort, EndPoint endPoint) {
            Debug.Log("Waiting for streaming server");
            byte[] data = new byte[1024];
            int receivedDate = scannedPort.ReceiveFrom(data, ref endPoint);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDate);
            Debug.Log($"received: {stringData} from: {endPoint}");
            string[] sanitizedString = stringData.Split(';');
            if(sanitizedString.First().Equals("Looking for Client")) {
                IdentifyRequestedTransmissions(sanitizedString.Last());
                supervisorAddress = FormattedIpAddress.ParseToAddress(endPoint.ToString());
                connectToServer = true;
            }
        }

        private void IdentifyRequestedTransmissions(string receivedMessage) {
            string[] formattedMessages = receivedMessage.Split(',');
            foreach(var message in formattedMessages) {
                string[] formattedMessage = message.Split(':');
                switch(formattedMessage[0]) {
                    case "WebStreaming":
                        requestedTransmissions.Add((ServerDiscovery.Transmission.WebStreaming,
                            FormattedIpAddress.ParseToAddress(formattedMessage[1])));
                        break;
                }
            }
        }
    }
}