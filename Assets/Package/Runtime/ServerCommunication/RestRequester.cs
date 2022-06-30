using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

namespace Package.Runtime.Communication {
    
    public class RestRequester{
        private readonly FormattedIpAddress ipAddress;

        private class AcceptAllCertificatesToIdentifyThumbprint : CertificateHandler  {
            public string identifiedThumbprint;
            
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                X509Certificate2 certificate = new X509Certificate2(certificateData);
                identifiedThumbprint = certificate.Thumbprint;
                return true;
            }
        }

        private class AcceptCertificateWithCertainThumbprint : CertificateHandler {
            private static string currentThumbprint;
            
            public static void SetCurrentThumbprint(string thumbprint) {
                currentThumbprint = thumbprint;
            }
            
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                X509Certificate2 certificate = new X509Certificate2(certificateData);
                return certificate.Thumbprint == currentThumbprint;
            }
        }

        public RestRequester(FormattedIpAddress restIpAddress) {
            ipAddress = restIpAddress;
        }

        public void IdentifySupervisor(Func<string, string, bool> onIdentification) {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://" + ipAddress + "/Settings");
            webRequest.certificateHandler = new AcceptAllCertificatesToIdentifyThumbprint();

            // Request and wait for the desired page.
            UnityWebRequestAsyncOperation requestAsyncOperation = webRequest.SendWebRequest();

            requestAsyncOperation.completed += _ => {
                switch(webRequest.result) {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        if(onIdentification(
                               ((AcceptAllCertificatesToIdentifyThumbprint)webRequest.certificateHandler).identifiedThumbprint,
                               webRequest.downloadHandler.text)
                           )
                            AcceptCertificateWithCertainThumbprint.SetCurrentThumbprint(
                                ((AcceptAllCertificatesToIdentifyThumbprint)webRequest.certificateHandler).identifiedThumbprint
                                );
                        break;
                }
                webRequest.Dispose();
            };
        }

        public void MakeGetRequest(string identifier, Action<string> onReception) {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://" + ipAddress + "/" + identifier);
            webRequest.certificateHandler = new AcceptCertificateWithCertainThumbprint();

            // Request and wait for the desired page.
            UnityWebRequestAsyncOperation requestAsyncOperation = webRequest.SendWebRequest();

            requestAsyncOperation.completed += _ => {
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
            webRequest.certificateHandler = new AcceptCertificateWithCertainThumbprint();

            // Request and wait for the desired page.
            UnityWebRequestAsyncOperation requestAsyncOperation = webRequest.SendWebRequest();

            requestAsyncOperation.completed += _ => {
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
            webRequest.certificateHandler = new AcceptCertificateWithCertainThumbprint();

            // Request and wait for the desired page.
            UnityWebRequestAsyncOperation requestAsyncOperation = webRequest.SendWebRequest();

            requestAsyncOperation.completed += _ => {
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