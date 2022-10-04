using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using de.jmu.ge.Gamification.General;
using GamesEngineering.QuestSystem.Core;
using Package.Runtime.Communication;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem {
    [Serializable]
    public class GameStateInfo {
        public string name;
        public Relation relation;
        public int targetValue;

        public GameStateInfo(string name, Relation relation, int targetValue) {
            this.name = name;
            this.relation = relation;
            this.targetValue = targetValue;
        }
    }
    
    [Serializable]
    public class Event {
        
        [SerializeField]
        private List<GameStateInfo> gameStateInfo;
        [SerializeField]
        private string onActivateMessage;
        [SerializeField]
        private bool repeated;

        private Dictionary<string, GameState> mappedGameStates;
        private Task currentTask;

        public Event(List<GameStateInfo> gameStateInfo, string onActivateMessage, bool repeated) {
            this.gameStateInfo = gameStateInfo;
            this.onActivateMessage = onActivateMessage; 
            this.repeated = repeated;
        }

        public void InitialiseTask(Dictionary<string, GameState> mappedGameStates, TaskManager taskManager) {
            this.mappedGameStates = mappedGameStates;
            currentTask = new Task();

            var logger = taskManager.transform.gameObject.AddComponent<SupervisorLogger>();
            logger.messageFormat = onActivateMessage;
            
            foreach(var info in gameStateInfo) {
                var currentCondition = new Condition();
                GameState.onValueChange += CreateValueAssertion(info, currentCondition);
                currentTask.Conditions.Add(currentCondition);
            }
            currentTask.OnTaskFinished.AddListener(logger.SendToSupervisor);
            
            taskManager.tasks.Add(currentTask);
        }

       

        private UnityAction CreateValueAssertion(GameStateInfo info, Condition condition)
            => info.relation switch {
                Relation.Lesser => () => {
                    Debug.Log("Is it smaller?");
                    condition.IsTrue = int.Parse(mappedGameStates[info.name].Value) < info.targetValue;
                },
                Relation.Equal => () => {
                    Debug.Log("Is it Equal?");
                    condition.IsTrue = int.Parse(mappedGameStates[info.name].Value) == info.targetValue;
                    Debug.Log(int.Parse(mappedGameStates[info.name].Value) == info.targetValue);
                },
                Relation.Greater => () => condition.IsTrue = int.Parse(mappedGameStates[info.name].Value) > info.targetValue,
                _ => null
            };
    }
}