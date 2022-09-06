using System.Collections.Generic;
using System.Linq;
using de.jmu.ge.Gamification.General;
using UnityEditor;
using UnityEngine;

namespace EventSystem {
    public enum Relation {
        Lesser,
        Equal,
        Greater
    }
    public class EventConfigurator : MonoBehaviour {
        //Copied from GamificationConfigurator. Sadly cannot reference it directly because it is Editor only
        private const string path = "Assets/Settings/de.jmu.ge.gamificationutils/States/";
        [SerializeField]
        private List<Event> events = new();

        public void AddEvent(Event @event) => events.Add(@event);

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
    }
}