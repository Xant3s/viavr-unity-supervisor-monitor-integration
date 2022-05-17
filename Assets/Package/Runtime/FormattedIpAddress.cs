using System.Net;

namespace Package.Runtime {
    public class FormattedIpAddress {
        public readonly IPAddress ipAddress;
        public readonly int port;

        public FormattedIpAddress(string address, int port) {
            ipAddress = IPAddress.Parse(address);
            this.port = port;
        }

        public static FormattedIpAddress ParseToAddress(string totalAddress) {
            string[] addressParts = totalAddress.Split(':');
            return new FormattedIpAddress(addressParts[0], int.Parse(addressParts[1]));
        }

        public override string ToString() {
            return $"{ipAddress}:{port}";
        }

        public string IpAddressToString() {
            return ipAddress.ToString();
        }
    }
}