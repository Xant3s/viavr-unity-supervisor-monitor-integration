using UnityEngine;

namespace Package.Runtime {
    public interface ServerTransmission {

        void StartTransmission(string receivedMessage, Transform gameObjectTransform);

        void StopTransmission();
    }
}