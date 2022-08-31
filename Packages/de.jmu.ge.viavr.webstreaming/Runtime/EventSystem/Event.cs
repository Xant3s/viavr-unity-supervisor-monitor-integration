using System;
using System.Collections.Generic;
using de.jmu.ge.Gamification.General;
using Package.Runtime.Communication;
using GamesEngineering.QuestSystem;

namespace EventSystem {
    public class GameStateRelation {
        private string name;
        private GameState gameState;
        private bool relation;
        private int targetValue;

        public GameStateRelation(string gameStateName, bool relation, int targetValue) {
            name = gameStateName;
            this.relation = relation;
            this.targetValue = targetValue;
        }

        public void SetCorrespondingGameState(GameState correspondingState) => gameState = correspondingState;
    }
    
    public class Event {
        private const string Identifier = "LogEvent";
        
        private readonly List<(string name, Relation relation, int targetValue)> gameStateInfo;
        private readonly Func<string> onActivateMessage;
        private readonly bool repeated;

        private string mappedGameStates;
        private Task currentTask;

        public Event(List<(string name, Relation relation, int targetValue)> gameStateInfo, string onActionMessage, bool repeated) {
            this.gameStateInfo = gameStateInfo;
            var messageParts = onActionMessage.Split('{','}');
            onActivateMessage += () => string.Concat(messageParts);
            this.repeated = repeated;
        }

        public void InitialiseTask(Dictionary<string, GameState> mappedGameStates) {
            this.mappedGameStates = this.mappedGameStates;
            currentTask = new Task();
            
            foreach(var info in gameStateInfo) {
                var currentCondition = new Condition();
                GameState.onValueChange += () => 
                    currentCondition.IsTrue = int.Parse(mappedGameStates[info.name].Value) >= info.targetValue;
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
                string.Join(" ", "[" + DateTime.Now.ToShortTimeString() + "]", onActivateMessage()));
    }
}