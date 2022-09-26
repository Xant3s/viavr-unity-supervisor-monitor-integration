using System;
using System.Net.Http;
using System.Timers;


namespace de.jmu.ge.viavr.webstreaming {
    public class WaitForConnectionRequest {
        public event EventHandler OnConnectionRequested;    // Supervisor asked to connect to this client.
        public event EventHandler OnConnectionDiscarded;    // Supervisor asked to connect to another client.
        private readonly Timer timer = new();
        private HttpClient client = new();
        private string identifier;
        private string baseAddress;


        public WaitForConnectionRequest(string baseAddress, string identifier) {
            this.baseAddress = baseAddress;
            this.identifier = identifier;
        }

        public void Start() {
            timer.Interval = 500;
            timer.Elapsed += ListenForConnectionRequest;
            timer.Start();
        }

        public void Stop() {
            timer.Stop();
        }

        private void ListenForConnectionRequest(object sender, ElapsedEventArgs e) => ListenForConnectionRequest();

        private async void ListenForConnectionRequest() {
            var content = await client.GetStringAsync($"{baseAddress}/clients/connected");
            if(content.Equals(string.Empty)) return;
            if(content.Equals(identifier)) {
                OnConnectionRequested?.Invoke(this, EventArgs.Empty);
            }
            else {
                OnConnectionDiscarded?.Invoke(this, EventArgs.Empty);
            }
            Stop();
        }
        
        ~WaitForConnectionRequest() => Stop();
    }
}