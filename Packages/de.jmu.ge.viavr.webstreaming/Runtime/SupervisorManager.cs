using System.Net;
using UnityEngine;

namespace de.jmu.ge.viavr.webstreaming {
    /// <summary>
    /// Establishes a connection to a supervisor monitor.
    /// </summary>
    public class SupervisorManager: MonoBehaviour {
        private readonly SupervisorDiscovery discovery = new();
        private string deviceName;
        private string operatingSystem;


        private void Awake() {
            deviceName = SystemInfo.deviceName;
            operatingSystem = SystemInfo.operatingSystem;
        }

        private void Start() {
            Debug.Log("Looking for supervisor");
            discovery.OnSupervisorFound += RegisterClient;
            discovery.Start();
        }

        private void RegisterClient(object sender, IPEndPoint endPoint) {
            Debug.Log($"Supervisor found at {endPoint.Address}:{endPoint.Port}");
            new RegisterClient().Register(endPoint, deviceName, operatingSystem);
        }
    }
}