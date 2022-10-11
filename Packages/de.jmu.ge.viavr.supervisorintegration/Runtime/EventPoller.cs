using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Events;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class EventPoller {
        private RestRequester restRequester;
        private Dictionary<string, UnityEvent<string>> eventHandler = new ();
        private int lastKnownEventId = -1;

        
        public void SetRestRequester(RestRequester requester) {
            restRequester = requester;
        }

        public void On(string eventName, Action<string> handler) {
            if(!eventHandler.ContainsKey(eventName)) {
                eventHandler.Add(eventName, new UnityEvent<string>());
            }
            eventHandler[eventName].AddListener(e => handler(e));
        }

        public async void PollEvents() {
            var content = await restRequester.Get("/events", $"newerThan={lastKnownEventId}");
            var events = JsonConvert.DeserializeObject<List<Event>>(content);
            if(events.Count == 0) return;
            lastKnownEventId = events.Last().id;
            foreach(var e in events) {
                if(eventHandler.ContainsKey(e.name)) {
                    eventHandler[e.name].Invoke(JsonConvert.SerializeObject(e));
                }
            }
        }
    }

    public struct Event {
        public int id;
        public string name;
        public string data;
    }
}