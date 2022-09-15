using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable ClassNeverInstantiated.Global

namespace Package.Runtime.Communication
{
    [Serializable]
    public class TransmissionInfo
    {
        public ServerDiscovery.Transmission Typ;
        public string Ip;
        public FormattedIpAddress FormattedIp;
    }
    
    [Serializable]
    public class ConnectionInfo {
        public string Id;
        
        public List<TransmissionInfo> RequestedTransmissions = new();
        
        public void OnAfterDeserialize()
        {
            foreach (var transmissionInfo in RequestedTransmissions)
            {
                transmissionInfo.FormattedIp = FormattedIpAddress.ParseToAddress(transmissionInfo.Ip);
            }
        }
        
        public static ConnectionInfo JsonifyConnectionInfo(string receivedMessage)
        {
            var currentConnectionInfo = JsonUtility.FromJson<ConnectionInfo>(receivedMessage);
            currentConnectionInfo?.OnAfterDeserialize();
            return currentConnectionInfo;
        }
    }
}