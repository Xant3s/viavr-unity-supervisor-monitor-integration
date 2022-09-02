using System;
using System.Collections.Generic;
using System.Text;
using de.jmu.ge.Gamification.General;
using Package.Runtime.Communication;
using GamesEngineering.QuestSystem;
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
        private const string Identifier = "LogEvent";
        
        private readonly List<(string name, Relation relation, int targetValue)> gameStateInfo;
        private readonly string onActivateMessage;
        private readonly bool repeated;

        private Dictionary<string, GameState> mappedGameStates;
        private Task currentTask;

        public Event(List<(string name, Relation relation, int targetValue)> gameStateInfo, string onActivateMessage, bool repeated) {
            this.gameStateInfo = gameStateInfo;
            this.onActivateMessage = onActivateMessage; 
            this.repeated = repeated;
        }

        public void InitialiseTask(Dictionary<string, GameState> mappedGameStates) {
            this.mappedGameStates = mappedGameStates;
            currentTask = new Task();
            
            foreach(var info in gameStateInfo) {
                var currentCondition = new Condition();
                GameState.onValueChange += CreateValueAssertion(info, currentCondition);
                currentTask.Conditions.Add(currentCondition);
            }
            
            currentTask.OnTaskFinished.AddListener(() => {
                SendToSupervisor();
                currentTask.IsActive = repeated;
            });

            currentTask.IsActive = true;
        }

        private void SendToSupervisor() => 
            RestRequester.GetInstance().MakePutRequest(
                Identifier,
                _ => {},
                string.Join(" ", "[" + DateTime.Now.ToShortTimeString() + "]", InferMessage(onActivateMessage)));

        // This works because C# always has one string before the split char and one afterwards.
        // But not really foolproof => Doesn't check for closing brackets... 
        private string InferMessage(string messageBlueprint) {
            var messageParts = messageBlueprint.Split('{','}');
            StringBuilder newMessage = new ();
            for(int partIndex = 0; partIndex < messageParts.Length; partIndex++) {
                if(partIndex % 2 == 0) {
                    newMessage.Append(mappedGameStates[messageParts[partIndex]]);
                }
                else {
                    newMessage.Append(messageParts[partIndex]);
                }
            }
            return newMessage.ToString();
        }

        private UnityAction CreateValueAssertion((string name, Relation relation, int targetValue) info, Condition condition)
            => info.relation switch {
                Relation.Lesser => () => condition.IsTrue = int.Parse(mappedGameStates[info.name].Value) < info.targetValue,
                Relation.Equal => () => condition.IsTrue = int.Parse(mappedGameStates[info.name].Value) == info.targetValue,
                Relation.Greater => () => condition.IsTrue = int.Parse(mappedGameStates[info.name].Value) > info.targetValue,
                _ => null
            };
    }
}