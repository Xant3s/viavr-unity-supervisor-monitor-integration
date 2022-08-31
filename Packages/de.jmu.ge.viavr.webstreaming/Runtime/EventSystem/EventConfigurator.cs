using System;
using System.Collections.Generic;
using System.Linq;
using de.jmu.ge.Gamification.General;
using UnityEditor;
using UnityEngine;
using WebStreaming;

namespace EventSystem {
    public enum Relation {
        Lesser,
        Equal,
        Greater
    }
    public class EventConfigurator : MonoBehaviour {
        //Copied from GamificationConfigurator. Sadly cannot reference it directly because it is Editor only
        private const string path = "Assets/Settings/de.jmu.ge.gamificationutils/States/";
        private List<Event> events = new();
        private List<(string name, Relation relation, int targetValue)> gameStateInfo;

        public void SetUpEvents(List<EventSettings> eventInfo) {
            foreach(var trackedEvent in eventInfo) {
                events.Add(new Event(ConvertGameStateInfo(trackedEvent.trackedGameStates),
                    trackedEvent.onActivateMessage,
                    bool.Parse(trackedEvent.repeated)));
            }
        }

        private void Start() {
            var gameStates = FindAllExistingGameStates();
            var mappedGameStates = MapGameStates(gameStates);

            foreach(var @event in events) {
                @event.InitialiseTask(mappedGameStates);
            }
        }

        private Dictionary<string, GameState> MapGameStates(List<GameState> gameStates)
            => gameStates.ToDictionary(state => state.name);
        
        private List<GameState> FindAllExistingGameStates() {
            var folders = new[] {path};
            var findAssets = AssetDatabase.FindAssets("t: GameState", folders);

            return findAssets.Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameState>).ToList();
        }

        private List<(string name, Relation relation, int targetValue)> ConvertGameStateInfo(List<GameStateSettings> gameStateInfos) =>
            gameStateInfos.Select(info => (info.gameStateName, Enum.Parse<Relation>(info.relation), int.Parse(info.targetValue)))
                .ToList();
    }
}