using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Menu Object")]
    [SerializeField]
    private GameObject mainMenu;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
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

    private void CloseMenu()
    {
        mainMenu.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    private void OpenMenu()
    {
        mainMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }
}
