using System;
using System.Collections.Generic;

namespace WebStreaming {
    [Serializable]
    public class WebStreamingSettings {
        public string active;
        public List<EventSettings> events;
    }

    [Serializable]
    public class EventSettings {
        public List<GameStateSettings> trackedGameStates;
        public string onActivateMessage;
        public string repeated;
    }

    [Serializable]
    public class GameStateSettings {
        public string gameStateName;
        public string relation;
        public string targetValue;
    }
}
