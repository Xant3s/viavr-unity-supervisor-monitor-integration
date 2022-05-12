using System;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [field:SerializeField]
    public Transform MainCamera { set; private get; }

    private void Awake() {
        if(Camera.main != null) MainCamera = Camera.main.transform;
    }

    void Update()
    {
        transform.position = MainCamera.position;
        transform.rotation = MainCamera.rotation;
    }
}
