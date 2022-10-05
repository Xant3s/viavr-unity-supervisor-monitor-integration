using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace de.jmu.ge.viavr.supervisorintegration {
    /// <summary>
    /// Establishes a connection to a supervisor monitor.
    /// </summary>
    public class SupervisorManager: MonoBehaviour {
        [SerializeField] private GameObject connectionPrompt;
        private RestRequester restRequester;
        private const int restPort = 3001;
        private IPAddress supervisorIPAddress;
        private string deviceName;


        private void Awake() {
            deviceName = SystemInfo.deviceName;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            ConnectToSupervisor();
        }
        
        private async void ConnectToSupervisor() {
            var discovery = new SupervisorDiscovery();
            await discovery.SupervisorFound();
            supervisorIPAddress = discovery.Address;
            restRequester = new RestRequester($"http://{supervisorIPAddress}:{restPort}");
            RegisterClient();
            var requestedClient = new WaitForRequest<int>(FetchRequestedClient, data => data >= 0);
            await requestedClient.WaitUntil();
            var anotherClientWasRequested = requestedClient.Result == 0;
            if(anotherClientWasRequested) return;
            ShowPrompt(supervisorIPAddress.ToString(), connectionPrompt);
        }

        private async Task<int> FetchRequestedClient() {
            var content = await restRequester.Get("/clients/connected");
            if(content.Equals(string.Empty)) return -1;
            return content.Equals(deviceName) ? 1 : 0;
        }

        private async void RegisterClient() => await supervisorintegration.RegisterClient.Register(restRequester);

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

        public async void AcceptSupervisor() => await restRequester.Post("/clients/accept");

        public async void RejectSupervisor() => await restRequester.Post("/clients/reject");

        public void StartStream() {
            var stream = new WebStreamingTransmission();
            stream.StartTransmission(supervisorIPAddress, transform);
        }

        public void StartKeepAlive() => InvokeRepeating(nameof(PostKeepAlive), 1, 5);

        private async void PostKeepAlive() {
            var response = await restRequester.Post("/clients/keep-alive");
            if(response.StatusCode != HttpStatusCode.OK) Debug.Log(response);
        }
    }
}