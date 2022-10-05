using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class RegisterClient {
        public async Task<HttpStatusCode> Register(IPEndPoint supervisorEndPoint, string deviceName, string operatingSystem) {
            var content = "{\"platform\": \"" + operatingSystem + "\", \"id\": \"" + deviceName + "\"}";
            var data = new StringContent(content, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"http://{supervisorEndPoint.Address}:3001");
            var response = await client.PostAsync("/clients/register", data);
            return response.StatusCode;
        }
    }
}