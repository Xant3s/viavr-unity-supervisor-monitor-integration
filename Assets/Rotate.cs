using UnityEngine;

public class Rotate : MonoBehaviour {
    [SerializeField] private float speed = 50;

    private void Update() => transform.Rotate(Vector3.up, speed * Time.deltaTime);
}