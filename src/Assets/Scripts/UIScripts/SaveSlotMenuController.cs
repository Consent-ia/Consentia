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
    [Header("First scene to start for a new game")]
    [SerializeField] private string startingSceneName = "Act1";

    public void NewGame()
    {
        if (!GameStateSaveSystem.Instance)
        {
            Debug.LogWarning("GameStateSaveSystem not found in scene.");
            return;
        }

        // Create a new slot id and set it for BOTH systems.
        string slotId = GameStateSaveSystem.CreateNewSlotId();
        GameStateSaveSystem.Instance.SetCurrentSlot(slotId);

        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.SetCurrentSlot(slotId);
            SaveSystem.Instance.ClearAllChoices();
        }

        // Save initial state (optional) then start the game.
        GameStateSaveSystem.Instance.SaveGameState();
        SceneTransitionManager.Instance.TransitionToScene(startingSceneName);
    }

    public void LoadGame()
    {
        if (!GameStateSaveSystem.Instance)
        {
            Debug.LogWarning("GameStateSaveSystem not found in scene.");
            return;
        }

        // Ensure the dialog choice system uses the same slot as the state system.
        string slotId = PlayerPrefs.GetString("CurrentSlotId", "");
        if (string.IsNullOrEmpty(slotId))
        {
            Debug.LogWarning("No CurrentSlotId found. Nothing to load.");
            return;
        }

        if (SaveSystem.Instance)
        {
            SaveSystem.Instance.SetCurrentSlot(slotId);
        }

        GameStateSaveSystem.Instance.LoadGameState(slotId);
    }
}

