using System.IO;
using de.jmu.ge.viavr.UnityBridge.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace de.jmu.ge.viavr.supervisorintegration.editor {
    public class SupervisorConfigurator : PackageConfigurator {
        public override void Init() {
            EnsureDirectory("Assets/Settings");
            EnsureDirectory("Assets/Resources");
            if(!File.Exists("Assets/Settings/BuildSettings.json")) return;
            File.Copy("Assets/Settings/BuildSettings.json", "Assets/Resources/BuildSettings.json", true);
        }

        public override void SetupScene() {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.supervisorintegration/Prefabs/Supervisor Manager.prefab");
            Assert.IsNotNull(prefab, "Supervisor Manager prefab not found");
            if(prefab == null) return;
            PrefabUtility.InstantiatePrefab(prefab);
        }

        private void EnsureDirectory(string path) {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}