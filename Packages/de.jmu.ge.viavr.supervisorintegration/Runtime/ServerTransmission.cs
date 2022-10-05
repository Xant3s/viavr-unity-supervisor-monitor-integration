using System.Net;
using UnityEngine;

namespace Package.Runtime.Communication {
    public interface ServerTransmission {

        void StartTransmission(IPAddress ipAddress, Transform gameObjectTransform);

        void StopTransmission();
    }
}