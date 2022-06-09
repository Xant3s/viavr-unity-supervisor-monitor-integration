using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class KeepAliveMessage {
        private Thread serverNotifier;
        private Thread serverAwaiter;
        private volatile bool timedOut;
        private volatile bool interruptedTimeOut;
        private Thread timeoutThread;
        private volatile Socket supervisorSocket;
        private EndPoint ep;
        private Action onDisconnect;
        private Action onTimeOut;

        public void StartMessaging(FormattedIpAddress supervisorAddress, Socket targetSocket, EndPoint targetEp) {
            timedOut = false;
            supervisorSocket = targetSocket;
            ep = targetEp;
            serverNotifier = new Thread(() => SendKeepAliveSignal(supervisorAddress));
            serverAwaiter = new Thread(AwaitSupervisorAliveSignal);
            serverAwaiter.Start();
            serverNotifier.Start();
        }

        public void StopMessaging() {
            serverNotifier?.Abort();
            serverAwaiter?.Abort();
            timeoutThread?.Abort();
        }

        public void AddOnDisconnect(Action disconnectFunction) {
            onDisconnect += disconnectFunction;
        }
        
        public void RemoveOnDisconnect(Action disconnectFunction) {
            onDisconnect -= disconnectFunction;
        }
        
        public void AddOnTimeOut(Action timeOutFunction) {
            onTimeOut += timeOutFunction;
        }
        
        public void RemoveOnTimeOut(Action timeOutFunction) {
            onTimeOut -= timeOutFunction;
        }

        private void SendKeepAliveSignal(FormattedIpAddress supervisorAddress) {
            IPEndPoint iep = new IPEndPoint(supervisorAddress.ipAddress, supervisorAddress.port);
            var udpClient = new UdpClient();
            while(!timedOut) {
                Debug.Log($"Sending keep alive message to {supervisorAddress}");
                byte[] sendBuffer = Encoding.ASCII.GetBytes("Still sharing");
                udpClient.Send(sendBuffer, sendBuffer.Length, iep);
                Thread.Sleep(5000);
            }
        }

        private void AwaitSupervisorAliveSignal() {
            while(!timedOut) {
                Debug.Log("Waiting for streaming server");
                byte[] data = new byte[1024];
                interruptedTimeOut = true;
                timeoutThread?.Interrupt();
                timeoutThread = new Thread(TimeOutTracker);
                interruptedTimeOut = false;
                timeoutThread.Start();
                bool correctMessage = false;
                while(!correctMessage) {
                    int receivedDate = supervisorSocket.ReceiveFrom(data, ref ep);
                    string stringData = Encoding.ASCII.GetString(data, 0, receivedDate);
                    if(stringData.Equals("Supervisor Monitor alive")) correctMessage = true;
                    else if(stringData.Equals("Disconnecting")) {
                        Debug.Log("Closing connection");
                        timeoutThread?.Abort();
                        serverNotifier?.Abort();
                        onDisconnect?.Invoke();
                        return;
                    }
                }
                interruptedTimeOut = true;
                timeoutThread?.Interrupt();
                interruptedTimeOut = false;
                Debug.Log($"Received keep alive signal from: {ep}");
            }
        }

        private void TimeOutTracker() {
            Thread.Sleep(15000);

            if(interruptedTimeOut) return;

            Debug.Log("Closing connection");
            timedOut = true;
            serverAwaiter?.Interrupt();
            serverNotifier?.Interrupt();
            onTimeOut.Invoke();
        }
    }
}