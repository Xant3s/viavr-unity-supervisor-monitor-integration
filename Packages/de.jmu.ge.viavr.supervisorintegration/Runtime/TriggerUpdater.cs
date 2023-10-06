using System.Linq;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    [RequireComponent(typeof(SupervisorManager))]
    public class TriggerUpdater: MonoBehaviour {
        private SupervisorManager supervisorManager;
        private TriggerManager triggerManager = new TriggerManager();
       
        
        private void Awake() {
            supervisorManager = GetComponent<SupervisorManager>();
        }
        
        private void Start() {
            triggerManager.FindTriggerSceneObjects();
            supervisorManager.onTriggerUpdate.AddListener(triggers => {
                triggerManager.ForEachTriggerSceneObject(triggers, (obj, triggerData) => {
                    var desiredValue = triggerData.triggerValue;
                    obj.transform
                        .Cast<Transform>()
                        .ToList()
                        .ForEach(child => child.gameObject.SetActive(child.name == desiredValue));
                });
            });
        }
    }
}