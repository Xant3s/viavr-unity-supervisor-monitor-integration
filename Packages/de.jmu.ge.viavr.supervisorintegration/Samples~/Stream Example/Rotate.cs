using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration.streamsample {
    public class Rotate : MonoBehaviour {
        [SerializeField] private float speed = 50;

        private void Update() => transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
