using System.Net;
using UnityEngine;

namespace de.jmu.ge.viavr.webstreaming {
    /// <summary>
    /// Establishes a connection to a supervisor monitor.
    /// </summary>
    public class SupervisorManager: MonoBehaviour {
        private readonly SupervisorDiscovery discovery = new();
        private WaitForConnectionRequest connection;
        private const int restPort = 3001;
        private string deviceName;
        private string operatingSystem;
        private string baseAddress;
        

        private void Awake() {
            deviceName = SystemInfo.deviceName;
            operatingSystem = SystemInfo.operatingSystem;
        }

        private void Start() {
            discovery.OnSupervisorFound += RegisterClient;
            discovery.OnSupervisorFound += WaitForConnectionRequest;
            discovery.OnSupervisorFound += StopDiscovery;
            discovery.Start();
        }

        private void RegisterClient(object sender, IPEndPoint endPoint) {
            Debug.Log($"Supervisor found at {endPoint.Address}:{endPoint.Port}");
            var supervisor = new IPEndPoint(endPoint.Address, restPort);
            new RegisterClient().Register(supervisor, deviceName, operatingSystem);
        }

        private void WaitForConnectionRequest(object sender, IPEndPoint endPoint) {
            Debug.Log("Start waiting for supervisor connection request.");
            connection = new WaitForConnectionRequest($"http://{endPoint.Address}:{restPort}", deviceName);
            connection.OnConnectionDiscarded += (_, _) => Debug.Log("discarded");
            connection.OnConnectionRequested += (_, _) => Debug.Log("requested");     
            connection.Start();
        }

        private void StopDiscovery(object _, IPEndPoint __) {
            discovery.Stop();
        }
        
        private void OnDestroy() {
            discovery.Stop();
            connection?.Stop();
        }
    }
}