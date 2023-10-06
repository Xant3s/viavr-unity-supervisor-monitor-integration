using System;
using de.jmu.ge.SpokeSceneImporter;
using de.jmu.ge.viavr.supervisorintegration.editor;
using UnityEngine;

// For testing purposes only
public class SetUuid : MonoBehaviour {
    public string uuid;
    
    
    private void Awake() {
        GetComponent<Uuid>().uuid = new Guid(uuid);
    }
}