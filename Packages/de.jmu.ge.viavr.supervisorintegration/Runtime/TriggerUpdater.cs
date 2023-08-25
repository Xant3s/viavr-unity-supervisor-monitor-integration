using System;
using System.Collections.Generic;
using System.Linq;
using de.jmu.ge.SpokeSceneImporter;
using Newtonsoft.Json;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    [RequireComponent(typeof(SupervisorManager))]
    public class TriggerUpdater: MonoBehaviour {
        private SupervisorManager supervisorManager;
        private readonly IDictionary<Guid, GameObject> triggerSceneObjects = new Dictionary<Guid, GameObject>();
       
        
        private void Awake() {
            supervisorManager = GetComponent<SupervisorManager>();
        }
        
        private void Start() {
            FindTriggerSceneObjects();
            supervisorManager.onTriggerUpdate.AddListener(triggers => {
                triggers.ForEach(triggerData => {
                    var obj = FindCorrespondingGameObject(triggerData);
                    if(obj == null) return;
                    var desiredValue = triggerData.triggerValue;
                    obj.transform
                        .Cast<Transform>()
                        .ToList()
                        .ForEach(child => child.gameObject.SetActive(child.name == desiredValue));
                });
            });
        }

        private GameObject FindCorrespondingGameObject(TriggerData triggerData) {
            var (_, obj) = triggerSceneObjects.FirstOrDefault(kvp
                => kvp.Key == new Guid(triggerData.sceneObject));
            return obj;
        }

        private void FindTriggerSceneObjects() {
            var allUuids = FindObjectsOfType<Uuid>();
            var buildSettingsText = BuildSettingsLoader.Load();
            dynamic buildSettings = JsonConvert.DeserializeObject(buildSettingsText);
            List<dynamic> floorMapNodes = buildSettings["floorMapConfig"]?["nodes"]?.ToObject<List<dynamic>>();
            if(floorMapNodes?.Count == 0) return;
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
    }
}