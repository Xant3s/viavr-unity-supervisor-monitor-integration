using UnityEngine;
using UnityEngine.UIElements;

public class QuitButton : MonoBehaviour {
    private void Awake() {
        GetComponent<UIDocument>().rootVisualElement
            .Q<Button>()
            .RegisterCallback<ClickEvent>(e => Application.Quit());
    }
}