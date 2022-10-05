using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class RestRequester {
        private readonly string restServerBaseAddress;
        private readonly HttpClient client;

        public RestRequester(string baseAddress) {
            restServerBaseAddress = baseAddress;
            client = new HttpClient();
            client.BaseAddress = new Uri(restServerBaseAddress);
        }

        public async Task<string> Get(string path) => await client.GetStringAsync($"{restServerBaseAddress}{path}");

        public async Task<HttpResponseMessage> Post(string path, string content = null) {
            var data = content != null ? new StringContent(content, Encoding.UTF8, "application/json") : null;
            return await client.PostAsync($"{restServerBaseAddress}{path}", data);
        }
    }
}