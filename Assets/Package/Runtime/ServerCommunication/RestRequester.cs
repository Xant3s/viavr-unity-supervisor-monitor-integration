using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

namespace Package.Runtime.Communication {
    
    // Based on https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning#.Net
    class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
    {
        // Encoded RSAPublicKey
        private static string PUB_KEY = 
            "30818902818100CBBD118EA8612A6CE47A69B0A179657F42F4FF9E5D3B88E60C43DBC24996F29CC617D876F805BC78FE42245F84C4BCB66DD4994D86DEEB4BFF1986A7182DFC923EAFC1815DC9F02B83F351497B1F676C388672875100511C09611D3F5A8C4E22DD35862FFFD62E19CED63ECAEFD42644E7EFCEA930165B5BE3E911EFA81CEC1D0203010001";
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

            webRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

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