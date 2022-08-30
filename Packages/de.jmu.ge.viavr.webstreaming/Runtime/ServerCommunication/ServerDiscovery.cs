using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Package.Runtime.ServerCommunication.ConnectionDialogue;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class ServerDiscovery : MonoBehaviour {
        public const string DisconnectMessage = "Disconnecting";

        public RestRequester restRequester;
        public EventPoller eventPoller;

        private volatile FormattedIpAddress supervisorAddress;
        private EndPoint ep;
        private volatile Socket supervisorSocket;
        private bool connected;

        private readonly List<ServerTransmission> transmissions = new();

        private PortScanner portScanner;
        private KeepAliveMessenger keepAliveMessenger;

        private ConnectionDialogueController prompt;

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

            prompt = GameObject.Find("ConnectionPrompt").GetComponent<ConnectionDialogueController>();
            prompt.gameObject.SetActive(false);

            portScanner.StartScanner(supervisorSocket, ep);
        }

        private void Update() {
            if(!portScanner.FoundServer(out FormattedIpAddress ipAddress)) return;
            FormattedIpAddress restAddress = new FormattedIpAddress(ipAddress.IpAddressToString(), 3001);
            
            prompt.SetOnConnectionAccepted(requestedTransmissionInfo =>
            {
                supervisorAddress = ipAddress;
                RestRequester.GetInstance().SetUpConnectionInfo(restAddress);
                eventPoller = transform.gameObject.AddComponent<EventPoller>();
                eventPoller.AddListener(Debug.Log);
                eventPoller.StartPolling();
                ConnectionInfo requestedTransmissions = ConnectionInfo.JsonifyConnectionInfo(requestedTransmissionInfo);
                foreach(var serverTransmission in requestedTransmissions.RequestedTransmissions) {
                    switch(serverTransmission.Typ) {
                        case Transmission.WebStreaming:
                            var webStreamer = new WebStreamingTransmission();
                            transmissions.Add(webStreamer);
                            webStreamer.StartTransmission(serverTransmission.FormattedIp, transform);
                            break;
                    }
                }
                connected = true;
                keepAliveMessenger.StartMessaging(supervisorAddress, supervisorSocket, ep);
            });
            prompt.SetOnConnectionDeclined(() => portScanner.StartScanner(supervisorSocket, ep));
            
            portScanner.SetConnected();
            
            RestRequester.IdentifySupervisor(restAddress, prompt);
            
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