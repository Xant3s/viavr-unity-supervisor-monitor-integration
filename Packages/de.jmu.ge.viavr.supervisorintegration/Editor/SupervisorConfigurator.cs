using System.IO;
using de.jmu.ge.viavr.UnityBridge.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace de.jmu.ge.viavr.supervisorintegration.editor {
    public class SupervisorConfigurator : PackageConfigurator {
        public override void Init() {
            EnsureDirectory("Assets/Settings");
            EnsureDirectory("Assets/Resources");
            if(!File.Exists("Assets/Settings/BuildSettings.json")) return;
            File.Copy("Assets/Settings/BuildSettings.json", "Assets/Resources/BuildSettings.json", true);
            InstantiateSupervisorManagerPrefab();
        }

        private void EnsureDirectory(string path) {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        private void InstantiateSupervisorManagerPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/de.jmu.ge.viavr.supervisorintegration/Prefabs/Supervisor Manager.prefab");
            if (prefab == null) return;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                PrefabUtility.InstantiatePrefab(prefab, scene);
            }
        }
    }
}