using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using de.jmu.ge.SpokeSceneImporter;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace de.jmu.ge.viavr.supervisorintegration {
    /// <summary>
    /// Establishes a connection to a supervisor monitor.
    /// </summary>
    public partial class SupervisorManager : MonoBehaviour {
        [SerializeField] private int eventPollRate = 1;
        [SerializeField] private int layoutPollRate = 5;
        [SerializeField] private GameObject connectionPrompt;
        [SerializeField] private UnityEvent supervisorCancelledConnectionRequest = new UnityEvent();
        [SerializeField] private TMP_Text supervisorAddress;
        [HideInInspector] public UnityEvent<List<TriggerData>> onTriggerUpdate = new UnityEvent<List<TriggerData>>();
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

        public async void ConnectToSupervisor() {
            var ipAddressString = Regex.Replace(supervisorAddress.text, @"\p{C}+", "");
            supervisorIPAddress = IPAddress.Parse(ipAddressString);
            RestRequester = new RestRequester($"http://{supervisorIPAddress}:{restPort}");
            eventPoller.SetRestRequester(RestRequester);
            await RegisterClient();
            await Authenticate();
            AcceptSupervisor();
            StartKeepAlive();
            StartLayoutSynchronization();
            StartPollEvents();
            StartPollingTriggerUpdates();
            StartPlayerSync();
            StartStream();
        }

        private async Task RegisterClient() => await supervisorintegration.RegisterClient.Register(RestRequester, uuid.ToString());

        private async Task Authenticate() {
            var response = await supervisorintegration.RegisterClient.Authenticate(RestRequester, uuid.ToString());
            var token = await response.Content.ReadAsStringAsync();
            RestRequester.Token = token;
        }

        public async void AcceptSupervisor() {
            await RestRequester.Post("/clients/accept");
            await RestRequester.Post("/clients/layout-model", SupervisorLayoutHandler.GetLayout());
            await RestRequester.Post("/clients/config", BuildSettingsLoader.Load());
            await RestRequester.Post("/trigger/level-bounds", CalculateLevelBounds());
        }

        public void StartStream() {
            var stream = new WebStreamingTransmission();
            stream.StartTransmission(supervisorIPAddress, transform);
        }

        private string CalculateLevelBounds() {
            var tags = GameObject.FindObjectsOfType<Tags>();
            var bottomLeft = tags.FirstOrDefault(tagsComponent => tagsComponent.tags.Any(tag => tag is "Level Boundary: Lower Left"))?.transform.position;
            var topRight = tags.FirstOrDefault(tagsComponent => tagsComponent.tags.Any(tag => tag is "Level Boundary: Upper Right"))?.transform.position;
            if(!bottomLeft.HasValue || !topRight.HasValue) return JsonConvert.SerializeObject(new LevelBounds());
            var levelBounds = new LevelBounds {
                minX = bottomLeft.Value.x,
                maxX = topRight.Value.x,
                minY = bottomLeft.Value.z,
                maxY = topRight.Value.z
            };
            var json = JsonConvert.SerializeObject(levelBounds);
            return json;
        }

        public void StartPollingTriggerUpdates() {
            InvokeRepeating(nameof(PollTriggerUpdates), 0, 0.5f);
        }

        private async void PollTriggerUpdates() {
            var triggerData = await RestRequester.Get("/trigger");
            var triggerDataList = JsonConvert.DeserializeObject<List<TriggerData>>(triggerData);
            onTriggerUpdate?.Invoke(triggerDataList);
        }

        public void StartKeepAlive() => InvokeRepeating(nameof(PostKeepAlive), 1, 5);

        public void StartPollEvents() => InvokeRepeating(nameof(PollEvents), 0, eventPollRate);
        
        public void StartPlayerSync() => InvokeRepeating(nameof(PostPlayerTransform), 0, 0.1f);

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

        private async void PostPlayerTransform() {
            var player = GameObject.FindWithTag("MainCamera").transform;
            var playerTransform = new PlayerTranform {
                x = player.position.x,
                y = player.position.z,
                rotation = player.rotation.eulerAngles.y
            };
            var json = JsonConvert.SerializeObject(playerTransform);
            await RestRequester.Post("/trigger/player-transform", json);
        }
    }
}