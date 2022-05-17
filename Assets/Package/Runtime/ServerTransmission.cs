using UnityEngine;

namespace Package.Runtime {
    public interface ServerTransmission {

        void StartTransmission(ServerDiscovery.FormattedIpAddress ipAddress, Transform gameObjectTransform);

        void StopTransmission();
    }
}