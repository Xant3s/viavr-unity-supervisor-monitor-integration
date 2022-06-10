using System;
using System.Collections.Generic;
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
    public class ConnectionInfo
    {
        public string ID;
        public List<TransmissionInfo> RequestedTransmissions = new();
        
        public void OnAfterDeserialize()
        {
            foreach (var transmissionInfo in RequestedTransmissions)
            {
                transmissionInfo.FormattedIp = FormattedIpAddress.ParseToAddress(transmissionInfo.Ip);
            }
        }
    }
}