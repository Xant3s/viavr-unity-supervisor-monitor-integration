using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebStreaming;

namespace EventSystem {
    public enum Relation {
        Lesser,
        Equal,
        Greater
    }
    public class EventConfigurator : MonoBehaviour {
        private List<Event> events = new();
        private List<(string name, Relation relation, int targetValue)> gameStateInfo;

        public void SetUpEvents(List<EventSettings> eventInfo) {
            foreach(var trackedEvent in eventInfo) {
                events.Add(new Event(ConvertGameStateInfo(trackedEvent.trackedGameStates),
                    trackedEvent.onActivateMessage,
                    bool.Parse(trackedEvent.repeated)));
            }
        }

        private List<(string name, Relation relation, int targetValue)> ConvertGameStateInfo(List<GameStateSettings> gameStateInfos) =>
            gameStateInfos.Select(info => (info.gameStateName, Enum.Parse<Relation>(info.relation), int.Parse(info.targetValue)))
                .ToList();
    }
}