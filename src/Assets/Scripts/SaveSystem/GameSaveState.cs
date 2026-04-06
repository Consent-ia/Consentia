using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateSaveSystem : MonoBehaviour
{
    public static GameStateSaveSystem Instance { get; private set; }

    [Header("Where to find the player at runtime")] [SerializeField]
    private string playerTag = "Player";

    // Which slot is active (one save file per "New Game")
    private string currentSlotId;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Persist across scenes so autosave/load and slot selection always works.
        // DontDestroyOnLoad(gameObject);

        // Load last used slot (optional)
        currentSlotId = PlayerPrefs.GetString("CurrentSlotId", "");
    }

    // ---------- Slot / Path ----------
    public void SetCurrentSlot(string slotId)
    {
        currentSlotId = slotId;
        PlayerPrefs.SetString("CurrentSlotId", currentSlotId);
        PlayerPrefs.Save();
    }

    public static string CreateNewSlotId()
    {
        // Easy unique id: timestamp
        return DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
    }

    private static string GetSlotPath(string slotId)
    {
        return Path.Combine(Application.persistentDataPath, $"save_state_{slotId}.json");
    }

    // ---------- Save ----------
    public void SaveGameState()
    {
        if (string.IsNullOrEmpty(currentSlotId))
        {
            Debug.LogWarning("No active slot id set. Not saving game state.");
            return;
        }

        var data = CaptureState();
        WriteToDisk(GetSlotPath(currentSlotId), data);
    }

    private GameStateData CaptureState()
    {
        var data = new GameStateData
        {
            sceneName = SceneManager.GetActiveScene().name
        };

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
        {
            Vector3 p = player.transform.position;
            data.playerX = p.x;
            data.playerY = p.y;
            data.playerZ = p.z;
        }

        // Progress integration
        if (ProgressManager.Instance)
        {
            data.interactedNpcNames = ProgressManager.Instance.GetInteractedNpcNames();
        }

        data.savedAtUtc = DateTime.UtcNow.ToString("o");
        return data;
    }

    private static void WriteToDisk(string path, GameStateData data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"Game state saved to: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game state: {e.Message}");
        }
    }

    // ---------- Load ----------
    public void LoadGameStateFromCurrentSlot()
    {
        if (string.IsNullOrEmpty(currentSlotId))
        {
            Debug.LogWarning("No active slot id set. Not loading game state.");
            return;
        }

        LoadGameState(currentSlotId);
    }

    public void LoadGameState(string slotId)
    {
        string path = GetSlotPath(slotId);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save state file found at: {path}");
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<GameStateData>(json);

            // Set slot as active
            SetCurrentSlot(slotId);

            // Load the scene, then apply player/progress after scene finishes loading
            void Handler(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != data.sceneName) return;
                SceneManager.sceneLoaded -= Handler;
                ApplyState(data);
            }

            SceneManager.sceneLoaded += Handler;

            SceneTransitionManager.Instance.TransitionToScene(data.sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game state: {e.Message}");
        }
    }

    private void ApplyState(GameStateData data)
    {
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
        }

        // Restore progress
        if (ProgressManager.Instance)
        {
            ProgressManager.Instance.SetInteractedNpcNames(data.interactedNpcNames);
        }

        Debug.Log($"Game state applied. Scene={data.sceneName} SavedAt={data.savedAtUtc}");
    }

    // ---------- Autosave hooks ----------
    private void OnApplicationQuit()
    {
        SaveGameState();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveGameState();
    }
}