using UnityEngine;

/// <summary>
/// Simple UI hook for Main Menu buttons:
/// - New Game: creates a new slot (separate save files) and starts at Act1
/// - Load Game: loads the last used slot (if any)
///
/// Wire these public methods to Button.onClick in the Inspector.
/// </summary>
public class SaveSlotMenuController : MonoBehaviour
{
    public static SaveSlotMenuController Instance { get; private set; }

    private const string startingSceneName = "Act1";

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        // Single-slot new game: fully overwrite prior progress.
        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.DeleteSaveFile();
        }

        if (GameSaveState.Instance)
        {
            GameSaveState.DeleteSaveFile();
        }

        ChangeScene.changeToScene(startingSceneName);
    }

    public void LoadGame()
    {
        if (!GameSaveState.Instance)
        {
            Debug.LogWarning("GameStateSaveSystem not found in scene.");
            return;
        }

        // Single-slot load.
        GameSaveState.Instance.LoadGameState();
    }
}

