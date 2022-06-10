using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class ServerDiscovery : MonoBehaviour {
        public const string DisconnectMessage = "Disconnecting";

        private volatile FormattedIpAddress supervisorAddress;
        private EndPoint ep;
        private volatile Socket supervisorSocket;
        private bool connected;

        private readonly List<ServerTransmission> transmissions = new();

        private PortScanner portScanner;
        private KeepAliveMessenger keepAliveMessenger;

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
            keepAliveMessenger = new KeepAliveMessenger();
            // Set up everything for the keep alive messenger
            keepAliveMessenger.AddOnDisconnect(OnLostConnection);
            keepAliveMessenger.AddOnTimeOut(OnLostConnection);

            portScanner.StartScanner(supervisorSocket, ep);
        }

        private void Update() {
            if(!portScanner.FoundServer(out FormattedIpAddress ipAddress, out ConnectionInfo newConnectionInfo)) return;
            if(Input.GetKeyDown("y")) {

                supervisorAddress = ipAddress;
                foreach(var serverTransmission in newConnectionInfo.RequestedTransmissions) {
                    switch(serverTransmission.Typ) {
                        case Transmission.WebStreaming:
                            var webStreamer = new WebStreamingTransmission();
                            transmissions.Add(webStreamer);
                            webStreamer.StartTransmission(serverTransmission.FormattedIp, transform);
                            break;
                    }
                }

                portScanner.SetConnected();
                connected = true;
                keepAliveMessenger.StartMessaging(supervisorAddress, supervisorSocket, ep);
            }
            else if(Input.GetKeyDown("n"))
            {
                portScanner.SetConnected();
                portScanner.StartScanner(supervisorSocket, ep);
            }

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
            if (supervisorAddress == null) return;
            IPEndPoint iep = new IPEndPoint(supervisorAddress.ipAddress, supervisorAddress.port);
            var udpClient = new UdpClient();
            Debug.Log($"Sending disconnect message to {supervisorAddress}");
            byte[] sendBuffer = Encoding.ASCII.GetBytes(DisconnectMessage);
            udpClient.Send(sendBuffer, sendBuffer.Length, iep);
        }

        private void BreakConnection() {
            keepAliveMessenger.StopMessaging();
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