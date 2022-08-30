using System;
using Package.Runtime.Communication;

namespace EventSystem {
    public class Event {
        private readonly string identifier;
        private readonly GameState trackedState;
        
        public Event(string identifier, GameState trackedState) {
            this.identifier = identifier;
            this.trackedState = trackedState;
        }

        public void Invoke() => SendToSupervisor(trackedState.FormatGameStateInfo());

        private void SendToSupervisor(string eventMessage) => 
            RestRequester.GetInstance().MakePutRequest(
                identifier,
                _ => {},
                string.Join(" ", DateTime.Now.ToShortTimeString(), eventMessage));
    }
}