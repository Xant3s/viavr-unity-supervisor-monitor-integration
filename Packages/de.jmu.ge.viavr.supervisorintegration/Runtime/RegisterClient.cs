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
        }
        
        public static async Task<HttpResponseMessage> Register(RestRequester requester, string uuid) {
            var deviceName = SystemInfo.deviceName;
            var operatingSystem = SystemInfo.operatingSystem;
            var content = new Content {
                uuid = uuid,
                friendlyName = deviceName,
                platform = operatingSystem
            };
            return await requester.Post("/clients/register", JsonConvert.SerializeObject(content));
        }
    }
}