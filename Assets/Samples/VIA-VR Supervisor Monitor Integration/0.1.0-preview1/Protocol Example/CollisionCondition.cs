using GamesEngineering.QuestSystem.Core;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    [RequireComponent(typeof(ConditionSwitch))]
    public class CollisionCondition : MonoBehaviour {
        private ConditionSwitch conditionSwitch;


        private void Awake() {
            conditionSwitch = GetComponent<ConditionSwitch>();
        }

        private void OnTriggerEnter(Collider other) {
            conditionSwitch.SetTrue();
        }
    }
}