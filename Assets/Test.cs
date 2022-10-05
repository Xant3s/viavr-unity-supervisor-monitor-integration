using System;
using de.jmu.ge.viavr.supervisorintegration;
using UnityEngine;

namespace DefaultNamespace {
    public class Test : MonoBehaviour {
        private void Start() {
            var manager = FindObjectOfType<SupervisorManager>();
            manager.EventPoller.On("test1", e => {
                Debug.Log(e);
            });
        }
    }
}