using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void Change()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string nextSceneName = currentScene switch
        {
            "IntroAct"  => "Act1",
            "Act1" => "Act2",
            "Act2" => "Act3",
            "Act3" => "Act4",
            "Act4" => "Act5",
            "Act5" => "Act6",
            _ => currentScene
        };
        
        changeToScene(nextSceneName);
    }

    public static void changeToScene(string sceneName)
    {
        if (SceneTransitionManager.Instance)
        {
            SceneTransitionManager.Instance.TransitionToScene(sceneName);
        }
        else
        {
            // Fallback if SceneTransitionManager is not found
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
