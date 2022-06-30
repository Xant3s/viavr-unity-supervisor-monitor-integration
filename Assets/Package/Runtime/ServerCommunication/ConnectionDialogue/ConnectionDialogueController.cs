using System.Linq;
using Package.Runtime.Communication;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Package.Runtime.ServerCommunication.ConnectionDialogue {
    public class ConnectionDialogueController : MonoBehaviour {
        [SerializeField] private Text header;
        [SerializeField] private Text body;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        private readonly UnityEvent<string> onAccept = new();
        private readonly UnityEvent onDecline = new();
        private readonly UnityEvent saveThumbPrint = new();

        private string requestedTransmissionInfo;

        public void Show(string id, string details) {
            header.text = $"Server ID: {id}";
            // ReSharper disable once IdentifierTypo
            var jsonfiedDetails = ConnectionInfo.JsonifyConnectionInfo(details);
            body.text = $"Server requires:\n{string.Join(", ", jsonfiedDetails.RequestedTransmissions.Select(transmission => transmission.Typ))}";
            requestedTransmissionInfo = details;
            gameObject.SetActive(true);
            acceptButton.onClick.AddListener(() => {
                gameObject.SetActive(false);
                saveThumbPrint.Invoke();
                onAccept.Invoke(requestedTransmissionInfo);
            });
            declineButton.onClick.AddListener(() => {
                gameObject.SetActive(false);
                onDecline.Invoke();
            });
        }

        public void SetOnConnectionDeclined(UnityAction onDeclined) {
            onDecline.RemoveAllListeners();
            onDecline.AddListener(onDeclined);
        }

        public void SetOnConnectionAccepted(UnityAction<string> onAccepted) {
            onAccept.RemoveAllListeners();
            onAccept.AddListener(onAccepted);
        }
        public void SaveThumbprint(UnityAction thumbPrintSaverCallback) {
            saveThumbPrint.RemoveAllListeners();
            saveThumbPrint.AddListener(thumbPrintSaverCallback);
        }
    }
}