using System;
using de.jmu.ge.SpokeSceneImporter;
using UnityEngine;

// For testing purposes only
public class SetUuid : MonoBehaviour {
    [SerializeField] private string uuid;
    
    
    private void Awake() {
        GetComponent<Uuid>().uuid = new Guid(uuid);
    }
}