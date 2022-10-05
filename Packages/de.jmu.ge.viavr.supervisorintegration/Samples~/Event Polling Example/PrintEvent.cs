using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    // Requries the supervior to be running and adding events to the EventStorage.
    // e.g. eventStorage.addEvent('test1', 42)
    public class PrintEvent : MonoBehaviour {
        private void Start() {
            var manager = FindObjectOfType<SupervisorManager>();
            manager.EventPoller.On("test1", e => {
                Debug.Log(e);
            });
        }
    }
}