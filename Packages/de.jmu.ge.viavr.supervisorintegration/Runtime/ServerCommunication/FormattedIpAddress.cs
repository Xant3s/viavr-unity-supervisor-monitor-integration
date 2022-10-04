using System.Net;

namespace Package.Runtime.Communication {
    public class FormattedIpAddress {
        public readonly IPAddress ipAddress;
        public readonly int port;

        public FormattedIpAddress(string address, int port) {
            ipAddress = IPAddress.Parse(address);
            this.port = port;
        }

        public static FormattedIpAddress ParseToAddress(string totalAddress) {
            string[] addressParts = totalAddress.Split(':');
            if(addressParts.Length >= 2) return new FormattedIpAddress(addressParts[0], int.Parse(addressParts[1]));
            return new FormattedIpAddress(addressParts[0], 0);
        }

        public override string ToString() {
            return $"{ipAddress}:{port}";
        }

        public string IpAddressToString() {
            return ipAddress.ToString();
        }
    }
}