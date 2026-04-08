using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Menu Object")]
    [SerializeField]
    private GameObject mainMenu;
    
    [Header("Landing Screen Settings Object")]
    [SerializeField]
    private GameObject landingScreenSettings;

    private void Awake()
    {
        // Singleton pattern
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CloseMenu();
    }

    public void ToggleMenu()
    {
        if (mainMenu.activeSelf)
        {
            CloseMenu();
            Debug.Log("Closing Menu");
        }
        else
        {
            OpenMenu();
            Debug.Log("Opening Menu");
        }
    }

    public void CloseMenu()
    {
        mainMenu.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    private void OpenMenu()
    {
        mainMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void ExitGame()
    {
        GameSaveState.SaveGameState();
        Application.Quit();
    }

    public void RestartGame()
    {
        SaveSlotMenuController.Instance.NewGame();
        CloseMenu();
    }

    public void LandingExit()
    {
        Application.Quit();
    }

    public void ToggleLandingSettings()
    {
        landingScreenSettings.SetActive(!landingScreenSettings.activeSelf);
    }
}
