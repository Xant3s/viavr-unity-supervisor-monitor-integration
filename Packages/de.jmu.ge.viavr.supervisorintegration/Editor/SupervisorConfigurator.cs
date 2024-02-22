using System.Collections.Generic;
using System.IO;
using System.Linq;
using de.jmu.ge.viavr.UnityBridge.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;


namespace de.jmu.ge.viavr.supervisorintegration.editor {
    public class SupervisorConfigurator : PackageConfigurator {
        public override void Init() {
            EnsureDirectory("Assets/Settings");
            EnsureDirectory("Assets/Resources");
            if(!File.Exists("Assets/Settings/BuildSettings.json")) return;
            File.Copy("Assets/Settings/BuildSettings.json", "Assets/Resources/BuildSettings.json", true);
        }

        public override void SetupScene() {
            SetupSupervisorManager();
            SpawnTriggers();
        }
        
        public static void SpawnTriggers() {
            var buildSettingsText = BuildSettingsLoader.Load();
            var buildSettings = JObject.Parse(buildSettingsText);
            if(buildSettings["triggers"] == null || buildSettings["triggers"]?.Count() == 0) return;
            var availableTriggersList = buildSettings["triggers"]?.ToObject<List<JToken>>();
            if(availableTriggersList == null || availableTriggersList.Count == 0) return;
            var floorMapTriggers = buildSettings["floorMapTriggers"]?.ToObject<List<JToken>>();
            if(floorMapTriggers == null || floorMapTriggers.Count == 0) return;
            var triggerDataList = floorMapTriggers.Select(trigger =>
                new TriggerData {
                    sceneObject = trigger["data"]?["sceneObject"]?.ToObject<string>(),
                    triggerType = trigger["data"]?["triggerType"]?.ToObject<string>(),
                    triggerValue = trigger["data"]?["triggerValue"]?.ToObject<string>(),
                    possibleValues = new string[] { }
                }).ToList();

            // For each trigger scene object spawn trigger prefabs
            var triggerManager = new TriggerManager();
            triggerManager.FindTriggerSceneObjects();
            triggerManager.ForEachTriggerSceneObject(triggerDataList, (obj, triggerData) => {
                var triggerInfo = availableTriggersList.FirstOrDefault(t => t["name"]?.ToObject<string>() == triggerData.triggerType);
                if(triggerInfo == null) return;
                var basePath = triggerInfo["path"]?.ToObject<string>();
                var valuesArray = triggerInfo["values"] as JArray;
                if(valuesArray != null) {
                    foreach (var valueToken in valuesArray) {
                        var value = valueToken.ToObject<string>();
                        var triggerAlt = Object.Instantiate(Resources.Load<GameObject>(Path.Combine(basePath, value)), obj.transform);
                        triggerAlt.SetActive(value == triggerData.triggerValue);
                        triggerAlt.name = value;
                    }
                }
            });
        }

        private static void SetupSupervisorManager() {
            if(Object.FindObjectOfType<SupervisorManager>()) return;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/de.jmu.ge.viavr.supervisorintegration/Prefabs/Supervisor Manager.prefab");
            Assert.IsNotNull(prefab, "Supervisor Manager prefab not found");
            if(prefab == null) return;
            PrefabUtility.InstantiatePrefab(prefab);
        }

        private static void EnsureDirectory(string path) {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}