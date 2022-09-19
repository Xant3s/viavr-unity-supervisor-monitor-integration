using System;
using UnityEngine;
using System.Net;
using System.Net.Http;
using System.Text;

namespace de.jmu.ge.viavr.webstreaming {
    public class RegisterClient: MonoBehaviour {
        public async void Register(IPEndPoint supervisorEndPoint) {
            Debug.Log("Registering client at " + supervisorEndPoint);
            var content = "{\"platform\": \"Android\", \"ID\": \"1\"}";
            var data = new StringContent(content, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"http://{supervisorEndPoint.Address}:3001");
            var response = await client.PostAsync("/clients/register", data);
            var result = response.Content.ReadAsStringAsync().Result;
            Debug.Log(result);
        }
    }
}