using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Package.Runtime.Communication {
    public class RestRequester : MonoBehaviour{

        private void Start() {
            MakeGetRequest("", text => Debug.Log("nReceived: " + text));
        }

        private const string ipAddress = "localhost:3000/";

        private static void MakeGetRequest(string identifier, Action<string> onReception) {
            UnityWebRequest webRequest = UnityWebRequest.Get(ipAddress + identifier);

            // Request and wait for the desired page.
            UnityWebRequestAsyncOperation requestAsyncOperation = webRequest.SendWebRequest();

            requestAsyncOperation.completed += operation => {
                switch(webRequest.result) {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        onReception(webRequest.downloadHandler.text);
                        break;
                }
                webRequest.Dispose();
            };
        }
    }
}