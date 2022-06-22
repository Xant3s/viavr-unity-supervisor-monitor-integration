using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Package.Runtime.Communication {
    public class RestRequester : MonoBehaviour{

        private void Start() {
            MakeGetRequest("", text => Debug.Log("nReceived: " + text));
        }

        private const string IPAddress = "localhost:3000/";

        public static void MakeGetRequest(string identifier, Action<string> onReception) {
            UnityWebRequest webRequest = UnityWebRequest.Get(IPAddress + identifier);

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
        
        public static void MakePostRequest(string identifier, Action<string> onReception) {
            WWWForm form = new WWWForm();
            UnityWebRequest webRequest = UnityWebRequest.Post(IPAddress + identifier, form);

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
        
        public static void MakePutRequest(string identifier, Action<string> onReception) {
            byte[] sampleData = System.Text.Encoding.UTF8.GetBytes("Test Bytes");
            UnityWebRequest webRequest = UnityWebRequest.Put(IPAddress + identifier, sampleData);

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