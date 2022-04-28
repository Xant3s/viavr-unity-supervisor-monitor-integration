using System;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [field:SerializeField]
    public Transform Origin { set; private get; }

    void Update()
    {
        transform.position = Origin.position;
        transform.rotation = Origin.rotation;
    }
}
