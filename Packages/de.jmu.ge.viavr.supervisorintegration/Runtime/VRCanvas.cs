using UnityEngine;

public class VRCanvas : MonoBehaviour {
    [SerializeField] private int distance = 300;
    [SerializeField] private float speed = 1f;
    private Transform cameraTransform;


    private void Start() {
        cameraTransform = UnityEngine.Camera.main.transform;
        transform.position = CalculatePosition();
    }

    private void Update() {
        MoveInFrontOfCamera();
        RotateTowardsCamera();
    }

    //TODO implement Coroutine with slerp to move the canvas

    private void MoveInFrontOfCamera() {
        var pos = CalculatePosition();
        var dist = Vector3.Distance(transform.position, pos);
        transform.position = Vector3.MoveTowards(transform.position, pos, speed * dist * Time.deltaTime);
    }

    private void RotateTowardsCamera() {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }

    private Vector3 CalculatePosition() {
        return cameraTransform.position + cameraTransform.forward * distance;
    }
}