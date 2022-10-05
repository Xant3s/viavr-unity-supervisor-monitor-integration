using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class PrintEvent : MonoBehaviour {
        private void Start() {
            var manager = FindObjectOfType<SupervisorManager>();
            manager.EventPoller.On("test1", e => {
                Debug.Log(e);
            });
        }
    }
}