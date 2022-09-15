using UnityEngine;

namespace Package.Runtime.Communication {
    public interface ServerTransmission {

        void StartTransmission(FormattedIpAddress ipAddress, Transform gameObjectTransform);

        void StopTransmission();
    }
}