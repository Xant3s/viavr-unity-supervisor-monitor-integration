using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class ServerDiscovery : MonoBehaviour {
        private volatile FormattedIpAddress supervisorAddress;
        private EndPoint ep;
        private volatile Socket supervisorSocket;
        private bool connected;

        private readonly List<ServerTransmission> transmissions = new();

        private PortScanner portScanner;
        private KeepAliveMessage keepAliveMessage;

        public enum Transmission {
            WebStreaming
        }

        private void Start() {
            portScanner = new PortScanner();
            // Set up everything for the port Scanner
            supervisorSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 41234);
            supervisorSocket.Bind(iep);
            ep = iep;
            keepAliveMessage = new KeepAliveMessage();
            // Set up everything for the keep alive messenger
            keepAliveMessage.AddOnDisconnect(OnLostConnection);
            keepAliveMessage.AddOnTimeOut(OnLostConnection);

            portScanner.StartScanner(supervisorSocket, ep);
        }

        private void Update() {
            if(!portScanner.FoundServer(out FormattedIpAddress ipAddress, out List<(Transmission, FormattedIpAddress)> requestedTransmissions)) return;
            supervisorAddress = ipAddress;
            foreach(var serverTransmission in requestedTransmissions) {
                switch(serverTransmission.Item1) {
                    case Transmission.WebStreaming:
                        var webStreamer = new WebStreamingTransmission();
                        transmissions.Add(webStreamer);
                        webStreamer.StartTransmission(serverTransmission.Item2, transform);
                        break;
                }
            }
            portScanner.SetConnected();
            connected = true;
            keepAliveMessage.StartMessaging(supervisorAddress, supervisorSocket, ep);
        }

        private void OnDestroy() {
            if(connected) SendDisconnectMessage();
            BreakConnection();
            StopTransmission();
        }

        private void OnLostConnection() {
            portScanner.StopScanner();
            StopTransmission();
            connected = false;
            portScanner.StartScanner(supervisorSocket, ep);

        }

        private void SendDisconnectMessage() {
            IPEndPoint iep = new IPEndPoint(supervisorAddress.ipAddress, supervisorAddress.port);
            var udpClient = new UdpClient();
            Debug.Log($"Sending disconnect message to {supervisorAddress}");
            byte[] sendBuffer = Encoding.ASCII.GetBytes("Disconnecting");
            udpClient.Send(sendBuffer, sendBuffer.Length, iep);
        }

        private void BreakConnection() {
            keepAliveMessage.StopMessaging();
            portScanner.StopScanner();
            supervisorSocket?.Close();
        }

        private void StopTransmission() {
            foreach(var dataTransmission in transmissions) {
                dataTransmission.StopTransmission();
            }
            transmissions.Clear();
        }
    }
}