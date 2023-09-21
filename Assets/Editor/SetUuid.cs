using System;
using de.jmu.ge.SpokeSceneImporter;
using de.jmu.ge.viavr.supervisorintegration.editor;
using UnityEditor;
using UnityEngine;

// For testing purposes only
public class SetUuid : MonoBehaviour {
    public string uuid;
    
    
    private void Awake() {
        GetComponent<Uuid>().uuid = new Guid(uuid);
    }

    [MenuItem("Test/Spawn Triggers")]
    public static void FixForEditMode() {
        foreach(var setUuid in FindObjectsOfType<SetUuid>()) {
            setUuid.gameObject.GetComponent<Uuid>().uuid = new Guid(setUuid.uuid);
        }
        SupervisorConfigurator.SpawnTriggers();
    }
}