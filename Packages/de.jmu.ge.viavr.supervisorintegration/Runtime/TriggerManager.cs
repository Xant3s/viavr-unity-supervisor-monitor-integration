using System;
using System.Collections.Generic;
using System.Linq;
using de.jmu.ge.SpokeSceneImporter;
using Newtonsoft.Json;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class TriggerManager {
        private readonly IDictionary<Guid, GameObject> triggerSceneObjects = new Dictionary<Guid, GameObject>();

        
        public void ForEachTriggerSceneObject(List<TriggerData> triggers, Action<GameObject, TriggerData> f) {
            if(triggers == null || triggers.Count == 0) return;
            triggers.ForEach(triggerData => {
                var obj = FindCorrespondingGameObject(triggerData);
                if(obj == null) return;
                f(obj, triggerData);
            });
        }
        
        public void FindTriggerSceneObjects() {
            var allUuids = GameObject.FindObjectsOfType<Uuid>();
            if(allUuids == null || allUuids.Length == 0) return;
            var buildSettingsText = BuildSettingsLoader.Load();
            if(string.IsNullOrWhiteSpace(buildSettingsText)) return;
            try {
                dynamic buildSettings = JsonConvert.DeserializeObject(buildSettingsText);
                List<dynamic> floorMapNodes = buildSettings?["floorMapConfig"]?["nodes"]?.ToObject<List<dynamic>>();
                if(floorMapNodes == null || floorMapNodes.Count < 2) return; // first node is floor map image
                floorMapNodes.RemoveAt(0);  // Floor map image node
                foreach(var node in floorMapNodes) {
                    string uuidString = node["data"]?["sceneObject"]?.ToObject<string>();
                    if(uuidString == null) continue;
                    var uuid = new Guid(uuidString);
                    var sceneObject = allUuids.FirstOrDefault(id => id.uuid == uuid)?.gameObject;
                    if(sceneObject == null) continue;
                    triggerSceneObjects.Add(uuid, sceneObject);
                }
            }
            catch(JsonException e) {
                Debug.Log(e);
            }
        }
        
        private GameObject FindCorrespondingGameObject(TriggerData triggerData) {
            var (_, obj) = triggerSceneObjects.FirstOrDefault(kvp
                => kvp.Key == new Guid(triggerData.sceneObject));
            return obj;
        }
    }
}