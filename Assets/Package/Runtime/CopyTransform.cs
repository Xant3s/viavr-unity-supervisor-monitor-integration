using System;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public Transform origin;

    /*private void Start()
    {
        if (Camera.main != null)
            origin = Camera.main.transform;
    }*/

    void Update()
    {
        transform.position = origin.position;
        transform.rotation = origin.rotation;
    }

    public void SetOrigin(Transform value)
    {
        origin = value;
    }
}
