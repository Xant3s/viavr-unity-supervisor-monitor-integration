using System;
using de.jmu.ge.SpokeSceneImporter;
using UnityEngine;

// For testing purposes only
public class SetUuid : MonoBehaviour {
    public string uuid;
    
    
    private void Awake() {
        var uuidComponent = GetComponent<Uuid>();
        uuidComponent.uuid = new Guid(uuid);
        uuidComponent.serializedUuid = uuid;
    }
}