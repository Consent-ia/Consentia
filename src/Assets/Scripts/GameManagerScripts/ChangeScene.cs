using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void Change()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string nextSceneName = currentScene switch
        {
            "Act1" => "Act2",
            "Act2" => "Act3",
            "Act3" => "Act4",
            "Act4" => "Act1",
            _ => currentScene
        };

        if (SceneTransitionManager.Instance)
        {
            SceneTransitionManager.Instance.TransitionToScene(nextSceneName);
        }
        else
        {
            // Fallback if SceneTransitionManager is not found
            Debug.LogWarning("SceneTransitionManager not found! Loading scene directly.");
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}
