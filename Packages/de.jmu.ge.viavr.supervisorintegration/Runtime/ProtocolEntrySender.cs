using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class ProtocolEntrySender : MonoBehaviour {
        public async void AddEntry(string content) {
            var restRequester = FindObjectOfType<SupervisorManager>()?.RestRequester;
            if(restRequester == null) {
                Debug.LogWarning("No RestRequester found");
                return;
            }
            await restRequester.Post("/protocol/add", "{\"content\":\"" + content + "\"}");
        }
    }
}