using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;


namespace de.jmu.ge.viavr.supervisorintegration {
    public class RegisterClient {
        public static async Task<HttpResponseMessage> Register(RestRequester requester) {
            var deviceName = SystemInfo.deviceName;
            var operatingSystem = SystemInfo.operatingSystem;
            var content = "{\"platform\": \"" + operatingSystem + "\", \"id\": \"" + deviceName + "\"}";
            return await requester.Post("/clients/register", content);
        }
    }
}