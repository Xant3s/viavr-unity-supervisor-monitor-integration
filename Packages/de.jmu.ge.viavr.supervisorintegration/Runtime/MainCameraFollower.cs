using UnityEngine;

public class MainCameraFollower : MonoBehaviour {
    private void Update() {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
    }
}