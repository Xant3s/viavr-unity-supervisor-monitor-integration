using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


namespace de.jmu.ge.viavr.supervisorintegration {
    public class RegisterClient {
        private class Content {
            public string uuid;
            public string friendlyName;
            public string platform;
            public string outdated;
        }
        
        public static async Task<HttpResponseMessage> Register(RestRequester requester, string uuid) {
            var clientInfo = CreateClientInfo(uuid);
            return await requester.Post("/clients/register", JsonConvert.SerializeObject(clientInfo));
        }

        public static async Task<HttpResponseMessage> Authenticate(RestRequester requester, string uuid) {
            var clientInfo = CreateClientInfo(uuid);
            return await requester.Post("/auth", JsonConvert.SerializeObject(clientInfo));
        }

        private static Content CreateClientInfo(string uuid) {
            var deviceName = SystemInfo.deviceName;
            var operatingSystem = SystemInfo.operatingSystem;
            var clientInfo = new Content {
                uuid = uuid,
                friendlyName = deviceName,
                platform = operatingSystem,
                outdated = false.ToString()
            };
            return clientInfo;
        }
    }
}