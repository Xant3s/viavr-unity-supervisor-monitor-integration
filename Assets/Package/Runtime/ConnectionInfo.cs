using System;
using System.Collections.Generic;
// ReSharper disable ClassNeverInstantiated.Global

namespace Package.Runtime
{
    [Serializable]
    public class TransmissionInfo
    {
        public string Typ;

        public string Ip;
    }
    
    [Serializable]
    public class ConnectionInfo
    {
        public string ID;

        public List<TransmissionInfo> RequestedTransmissions = new();
    }
}