using System;
using de.jmu.ge.SpokeSceneImporter;
using UnityEngine;

// For testing purposes only
public class SetUuid : MonoBehaviour {
    private void Awake() {
        GetComponent<Uuid>().uuid = new Guid("2266BED7-6CC4-4866-95DD-9BCD3CF9EAFC");
    }
}