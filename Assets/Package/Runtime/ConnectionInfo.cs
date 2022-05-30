using System.Collections.Generic;

namespace Package.Runtime
{
    public class ConnectionInfo
    {
        public string ID;

        public List<(string transmissionType, string ipAddress)> RequestedTransmissions;
    }
}