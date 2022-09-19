using System;
using UnityEngine;
using System.Net;
using System.Net.Http;
using System.Text;

namespace de.jmu.ge.viavr.webstreaming {
    public class RegisterClient {
        public async void Register(IPEndPoint supervisorEndPoint, string deviceName, string operatingSystem) {
            Debug.Log("Registering client at " + supervisorEndPoint);
            var content = "{\"platform\": \"" + operatingSystem + "\", \"id\": \"" + deviceName + "\"}";
            var data = new StringContent(content, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"http://{supervisorEndPoint.Address}:3001");
            var response = await client.PostAsync("/clients/register", data);
            var result = response.Content.ReadAsStringAsync().Result;
            Debug.Log(result);
        }
    }
}