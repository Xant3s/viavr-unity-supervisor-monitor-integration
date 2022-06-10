using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
            body.text = $"Server requires:{details}";
            gameObject.SetActive(true);
        }

        public void SetOnConnectionDeclined(UnityAction onDeclined) {
            declineButton.onClick.RemoveAllListeners();
            declineButton.onClick.AddListener(() => {
                gameObject.SetActive(false);
                onDeclined.Invoke();
            });
        }

        public void SetOnConnectionAccepted(UnityAction onAccepted) {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() => {
                gameObject.SetActive(false);
                onAccepted.Invoke();
            });
        }
    }
}