using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class MoveRight : MonoBehaviour {
        [SerializeField] private float speed = 1f;
        
        private void Update() {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}