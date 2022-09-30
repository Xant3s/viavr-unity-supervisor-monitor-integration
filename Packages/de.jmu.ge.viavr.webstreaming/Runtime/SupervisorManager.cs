using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace de.jmu.ge.viavr.webstreaming {
    /// <summary>
    /// Establishes a connection to a supervisor monitor.
    /// </summary>
    public class SupervisorManager: MonoBehaviour {
        [SerializeField] private GameObject connectionPrompt;
        private const int restPort = 3001;
        private string deviceName;
        private string operatingSystem;
        private string baseAddress;
        

        private void Awake() {
            deviceName = SystemInfo.deviceName;
            operatingSystem = SystemInfo.operatingSystem;
        }

        private void Start() {
            ConnectToSupervisor();
        }
        
        private async void ConnectToSupervisor() {
            var discovery = new SupervisorDiscovery();
            await discovery.SupervisorFound();
            var address = discovery.Address;
            baseAddress = $"http://{address}:{restPort}";
            RegisterClient(address);
            var requestedClient = new WaitForRequest<int>(FetchRequestedClient, data => data >= 0);
            await requestedClient.WaitUntil();
            var anotherClientWasRequested = requestedClient.Result == 0;
            if(anotherClientWasRequested) return;
            ShowPrompt(baseAddress, connectionPrompt);
        }

        private async Task<int> FetchRequestedClient() {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            var content = await client.GetStringAsync($"{baseAddress}/clients/connected");
            if(content.Equals(string.Empty)) return -1;
            return content.Equals(deviceName) ? 1 : 0;
        }

        private void RegisterClient(IPAddress address) {
            Debug.Log($"Supervisor found at {address}");
            var supervisor = new IPEndPoint(address, restPort);
            new RegisterClient().Register(supervisor, deviceName, operatingSystem);
        }

        private void ShowPrompt(string address, GameObject prompt) {
            try {
                prompt.transform.GetChild(0).Find("Body").GetComponent<Text>().text = $"Do you want to allow {address} to supervise your session?";
                prompt.SetActive(true);
            }
            catch(Exception e) {
                Debug.Log(e);
                throw;
            }
        }

        public async void AcceptSupervisor() {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            var response = await client.PostAsync("/clients/accept", null);
            var result = response.Content.ReadAsStringAsync().Result;
            Debug.Log(result);
        }

        public async void RejectSupervisor() {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            var response = await client.PostAsync("/clients/reject", null);
            var result = response.Content.ReadAsStringAsync().Result;
            Debug.Log(result);
        }
    }
}