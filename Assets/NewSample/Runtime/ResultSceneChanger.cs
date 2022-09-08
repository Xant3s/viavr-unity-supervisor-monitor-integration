using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneChanger : MonoBehaviour {

    public void ChangeScene() {
        SceneManager.LoadScene("NewSample/Scenes/ResultScene");
    }
}
