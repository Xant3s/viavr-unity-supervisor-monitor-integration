using System;
using System.Collections.Generic;

namespace EventSystem {
    public abstract class GameState {
        private Dictionary<string, Func<string>> trackedVariables = new();

        public void AddTrackedVariable(string trackingName, Func<string> valueCallback) 
            => trackedVariables.Add(trackingName, valueCallback);

        public abstract string FormatGameStateInfo();

    }
}