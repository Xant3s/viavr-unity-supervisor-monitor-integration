using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Package.Runtime.ServerCommunication.ConnectionDialogue;
using Random = UnityEngine.Random;

namespace Package.Runtime.Communication {

    public class RestRequester {
        private FormattedIpAddress ipAddress;
        private static RestRequester requester;

        private readonly List<(string identifier, Action<string> onReception)> getMessageBacklog = new();
        private readonly List<(string identifier, Action<string> onReception, WWWForm postForm)> postMessageBacklog = new();
        private readonly List<(string identifier, Action<string> onReception, string putMessage)> putMessageBacklog = new();

        private bool IsNotSetUp => ipAddress == null;

        private class AcceptAllCertificatesToIdentifyThumbprint : CertificateHandler {
            public string identifiedThumbprint;

            protected override bool ValidateCertificate(byte[] certificateData) {
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

            protected override bool ValidateCertificate(byte[] certificateData) {
                X509Certificate2 certificate = new X509Certificate2(certificateData);
                return certificate.Thumbprint == currentThumbprint;
            }
        }

        private RestRequester() {
            // Empty
        }

        public static RestRequester GetInstance() {
            return requester ??= new RestRequester();
        }

        public static void IdentifySupervisor(FormattedIpAddress restIpAddress, ConnectionDialogueController identificationHandler) {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://" + restIpAddress + "/Settings/" + GenerateId());
            webRequest.certificateHandler = new AcceptAllCertificatesToIdentifyThumbprint();

            HandleWebRequest(webRequest, _ => {
                string newThumbprint = ((AcceptAllCertificatesToIdentifyThumbprint)webRequest.certificateHandler).identifiedThumbprint;
                identificationHandler.Show(newThumbprint, webRequest.downloadHandler.text);
                identificationHandler.SaveThumbprint(() => AcceptCertificateWithCertainThumbprint.SetCurrentThumbprint(newThumbprint));
            });
        }

        public void SetUpConnectionInfo(FormattedIpAddress restIpAddress) {
            ipAddress = restIpAddress;
            foreach(var getRequest in getMessageBacklog) 
                MakeGetRequest(getRequest.identifier, getRequest.onReception);
            foreach(var postRequest in postMessageBacklog) 
                MakePostRequest(postRequest.identifier, postRequest.onReception, postRequest.postForm);
            foreach(var putRequest in putMessageBacklog) 
                MakePutRequest(putRequest.identifier, putRequest.onReception, putRequest.putMessage);
        }

        public void MakeGetRequest(string identifier, Action<string> onReception) {
            if(IsNotSetUp) {
                getMessageBacklog.Add((identifier,onReception));
                return;
            }
            UnityWebRequest webRequest = UnityWebRequest.Get("https://" + ipAddress + "/" + identifier);
            webRequest.certificateHandler = new AcceptCertificateWithCertainThumbprint();
            HandleWebRequest(webRequest, onReception);
        }

        public void MakePostRequest(string identifier, Action<string> onReception, WWWForm postForm) {
            if(IsNotSetUp) {
                postMessageBacklog.Add((identifier,onReception,postForm));
                return;
            }
            UnityWebRequest webRequest = UnityWebRequest.Post("https://" + ipAddress + "/" + identifier, postForm);
            webRequest.certificateHandler = new AcceptCertificateWithCertainThumbprint();
            HandleWebRequest(webRequest, onReception);
        }

        public void MakePutRequest(string identifier, Action<string> onReception, string putMessage) {
            if(IsNotSetUp) {
                putMessageBacklog.Add((identifier,onReception,putMessage));
                return;
            }
            byte[] dataMessage = Encoding.UTF8.GetBytes(putMessage);
            UnityWebRequest webRequest = UnityWebRequest.Put("https://" + ipAddress + "/" + identifier, dataMessage);
            webRequest.certificateHandler = new AcceptCertificateWithCertainThumbprint();
            HandleWebRequest(webRequest, onReception);
        }

        private static string GenerateId() {
            StringBuilder idBuilder = new();

            for(int index = 0; index < 6; index++) {
                int randomNumber = Random.Range(0, 36);
                if(randomNumber >= 10) {
                    idBuilder.Append(Convert.ToChar(65 + (randomNumber - 10)));
                }
                else {
                    idBuilder.Append(randomNumber);
                }
            }
            return idBuilder.ToString();
        }

        private static void HandleWebRequest(UnityWebRequest webRequest, Action<string> onReception) {
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