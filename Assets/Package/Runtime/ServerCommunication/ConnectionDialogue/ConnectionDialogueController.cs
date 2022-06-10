using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

namespace Package.Runtime.ServerCommunication.ConnectionDialogue {
    public class ConnectionDialogueController : MonoBehaviour {
        [SerializeField] private Text header;
        [SerializeField] private Text body;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;
        
        private UnityAction onAccept;
        private UnityAction onDecline;

        public void Show(string id, string details) {
            header.text = $"Server ID: {id}";
            body.text = $"Server requires:";
            gameObject.SetActive(true);
        }

        public void AddOnConnectionDeclined() {
            gameObject.SetActive(false);
            acceptButton.clicked += () => onAccept?.Invoke();
        }

        public void AddOnConnectionAccepted() {
            gameObject.SetActive(false);
            declineButton.clicked += () => onDecline?.Invoke();
        }
    }
}