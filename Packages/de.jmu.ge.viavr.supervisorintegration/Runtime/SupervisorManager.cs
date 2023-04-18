using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace de.jmu.ge.viavr.supervisorintegration {
    /// <summary>
    /// Establishes a connection to a supervisor monitor.
    /// </summary>
    public class SupervisorManager : MonoBehaviour {
        [SerializeField] private int eventPollRate = 1;
        [SerializeField] private int layoutPollRate = 5;
        [SerializeField] private GameObject connectionPrompt;
        private EventPoller eventPoller = new();
        private const int restPort = 3001;
        private const float registerTimer = 5f;
        private IPAddress supervisorIPAddress;
        private Guid uuid = Guid.NewGuid();

        public EventPoller EventPoller => eventPoller;
        public RestRequester RestRequester { get; private set; }


        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            ConnectToSupervisor();
        }

        private async void ConnectToSupervisor() {
            var discovery = new SupervisorDiscovery();
            await discovery.SupervisorFound();
            supervisorIPAddress = discovery.Address;
            RestRequester = new RestRequester($"http://{supervisorIPAddress}:{restPort}");
            eventPoller.SetRestRequester(RestRequester);
            InvokeRepeating(nameof(RegisterClient), 0f, registerTimer);
            TryToConnectToSupervisor();
        }
        
        public async void TryToConnectToSupervisor() {
            // TODO: if supervisor cancels, hide prompt and start over
            await Task.Delay(2000); // Wait for supervisor to clear connected client.
            var requestedClient = new WaitForRequest<int>(FetchRequestedClient, data => data >= 0);
            await requestedClient.WaitUntil();
            var anotherClientWasRequested = requestedClient.Result == 0;
            if(anotherClientWasRequested) return;
            await Authenticate();
            ShowPrompt(supervisorIPAddress.ToString(), connectionPrompt);
        }

        public void StopLookingForSupervisor() => CancelInvoke(nameof(RegisterClient));

        private async Task<int> FetchRequestedClient() {
            var content = await RestRequester.Get("/clients/connected");
            if(content.Equals(string.Empty)) return -1;
            return content.Equals(uuid.ToString()) ? 1 : 0;
        }

        private async void RegisterClient() => await supervisorintegration.RegisterClient.Register(RestRequester, uuid.ToString());

        private async Task Authenticate() {
            var response = await supervisorintegration.RegisterClient.Authenticate(RestRequester, uuid.ToString());
            var token = await response.Content.ReadAsStringAsync();
            RestRequester.Token = token;
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
            await RestRequester.Post("/clients/accept");
            await RestRequester.Post("/clients/layout-model", SupervisorLayoutHandler.GetLayout());
        }

        public async void RejectSupervisor() => await RestRequester.Post("/clients/reject");

        public void StartStream() {
            var stream = new WebStreamingTransmission();
            stream.StartTransmission(supervisorIPAddress, transform);
        }

        public void StartKeepAlive() => InvokeRepeating(nameof(PostKeepAlive), 1, 5);

        public void StartPollEvents() => InvokeRepeating(nameof(PollEvents), 0, eventPollRate);

        public void StartLayoutSynchronization() => InvokeRepeating(nameof(GetLayoutConfig), layoutPollRate, layoutPollRate);

        private void PollEvents() => eventPoller.PollEvents();

        private async void PostKeepAlive() {
            var response = await RestRequester.Post("/clients/keep-alive");
            if(response.StatusCode != HttpStatusCode.OK) Debug.Log(response);
        }

        private async void GetLayoutConfig() {
            var layout = await RestRequester.Get("/clients/layout-Config");
            SupervisorLayoutHandler.SaveLayout(layout);
        }
    }
}