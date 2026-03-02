using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    string nextSceneName = "Act1";
    public void Change()
    {
        if (SceneManager.GetActiveScene().name == "Act1")
        {
            nextSceneName = "Act2";
        }
        else if (SceneManager.GetActiveScene().name == "Act2")
        {
            nextSceneName = "Act1";
        }
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        PlayerManager.Instance.SetPlayerPosition(nextSceneName);
    }
}
