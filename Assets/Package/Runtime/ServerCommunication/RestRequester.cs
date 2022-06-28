using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

namespace Package.Runtime.Communication {
    
    class AcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();
            /*if (pk.ToLower().Equals(PUB_KEY.ToLower()))
                return true;
            return false;*/
            return true;
        }
    }
    
    public class RestRequester{
        private readonly FormattedIpAddress ipAddress;

        public RestRequester(FormattedIpAddress restIpAddress) {
            ipAddress = restIpAddress;
        }

        public void MakeGetRequest(string identifier, Action<string> onReception) {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://" + ipAddress + "/" + identifier);
            webRequest.certificateHandler = new AcceptAllCertificates();

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
        
        public void MakePostRequest(string identifier, Action<string> onReception, WWWForm postForm) {
            UnityWebRequest webRequest = UnityWebRequest.Post("https://" + ipAddress + "/" + identifier, postForm);
            webRequest.certificateHandler = new AcceptAllCertificates();

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
        
        public void MakePutRequest(string identifier, Action<string> onReception, string putMessage) {
            byte[] dataMessage = System.Text.Encoding.UTF8.GetBytes(putMessage);
            UnityWebRequest webRequest = UnityWebRequest.Put("https://" + ipAddress + "/" + identifier, dataMessage);
            webRequest.certificateHandler = new AcceptAllCertificates();

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