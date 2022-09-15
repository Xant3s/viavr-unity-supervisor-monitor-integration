using System;
using System.Collections;
using UnityEngine;

namespace Package.Runtime.Communication {
    public class EventPoller : MonoBehaviour {
        private Coroutine pollCoroutine;
        private Action<string> onEvent;

        public void StartPolling() {
            pollCoroutine = StartCoroutine(PollCoroutine());
        }
        
        public void StopPolling() {
            StopCoroutine(pollCoroutine);
        }

        public void AddListener(Action<string> onReceiveEvent) {
            onEvent += onReceiveEvent;
        }

        public void RemoveListener(Action<string> onReceiveEvent) {
            onEvent -= onReceiveEvent;
        }

        private IEnumerator PollCoroutine() {
            while(true) {
                RestRequester.GetInstance().MakeGetRequest("Events", getMessage => {
                    if(getMessage.Equals("[]")) 
                        return;
                    onEvent?.Invoke(getMessage);
                    });
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}