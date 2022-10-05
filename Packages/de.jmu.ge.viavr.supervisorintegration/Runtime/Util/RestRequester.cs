using System;
using System.Collections.Generic;
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

        public async Task<HttpResponseMessage> Post(string path, string content = null) {
            var data = content != null ? new StringContent(content, Encoding.UTF8, "application/json") : null;
            return await client.PostAsync($"{restServerBaseAddress}{path}", data);
        }

        public async Task<string> Get(string path, params string[] parameters) {
            var requestUri = $"{restServerBaseAddress}{path}";
            requestUri = AddParamsToRequestUri(requestUri, parameters);
            return await client.GetStringAsync(requestUri);
        }

        private static string AddParamsToRequestUri(string requestUri, IReadOnlyList<string> parameters) {
            if(parameters.Count > 0) {
                requestUri += "?";
                for(var i = 0; i < parameters.Count; i++) {
                    requestUri += parameters[i];
                    if(i < parameters.Count - 1) {
                        requestUri += "&";
                    }
                }
            }
            return requestUri;
        }
    }
}