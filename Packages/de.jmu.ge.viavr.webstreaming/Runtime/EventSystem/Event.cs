using System;
using Package.Runtime.Communication;

namespace EventSystem {
    public abstract class Event {
        private readonly string identifier;
        
        protected Event(string identifier) {
            this.identifier = identifier;
        }

        public void Invoke() => SendToSupervisor(ReceiveEventInfo());
        
        protected abstract string ReceiveEventInfo();

        private void SendToSupervisor(string eventMessage) => 
            RestRequester.requester.MakePutRequest(
                identifier,
                _ => {},
                string.Join(" ", DateTime.Now.ToShortTimeString(), eventMessage));
    }
}