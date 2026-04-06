using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSaveState : MonoBehaviour
{
    public static GameSaveState Instance { get; private set; }

    private const string playerTag = "Player";

    private GameObject player;

    // Single-slot overwrite save (always one file at a time)
    private const string SaveFileName = "saveState.json";
    
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

    // ---------- Path ----------
    private static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    /// <summary>
    /// Deletes the current game-state save file.
    /// Call this when starting a New Game to fully overwrite prior progress.
    /// </summary>
    public static void DeleteSaveFile()
    {
        string path = GetSavePath();
        if (!File.Exists(path)) return;
        File.Delete(path);
        Debug.Log($"Deleted game state save: {path}");
    }

    // ---------- Save ----------
    private static void SaveGameState()
    {
        var data = CaptureState();
        WriteToDisk(GetSavePath(), data);
    }

    private static GameStateData CaptureState()
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
    public void LoadGameState()
    {
        string path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save state file found at: {path}");
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<GameStateData>(json);

            // Robust load pipeline: transition scene first, then wait until runtime
            // objects exist and have run their own OnSceneLoaded initializers.
            StartCoroutine(LoadPipelineCoroutine(data));
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game state: {e.Message}");
        }
    }

    private IEnumerator LoadPipelineCoroutine(GameStateData data)
    {
        // Kick off scene transition.
        ChangeScene.changeToScene(data.sceneName);

        // Wait until the correct scene is active.
        while (SceneManager.GetActiveScene().name != data.sceneName)
            yield return null;

        // Wait a couple frames so other sceneLoaded handlers (PlayerManager/ProgressManager)
        // run and finish their own initialization.
        yield return null;
        yield return null;

        // Wait for player & progress manager to exist.
        float timeout = 5f;
        while (timeout > 0f)
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
            if (player && ProgressManager.Instance) break;
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        ApplyState(data);

        // Apply again at end-of-frame to defeat any late spawn/teleport scripts.
        yield return new WaitForEndOfFrame();
        ApplyState(data);
    }

    private void ApplyState(GameStateData data)
    {
        var loadedPos = new Vector3(data.playerX, data.playerY);
        PlayerManager.SetNextLoadedPlayerPosition(loadedPos);

        if (player)
        {
            player.transform.position = loadedPos;
        }

        // Restore progress
        if (ProgressManager.Instance)
        {
            ProgressManager.Instance.SetInteractedNpcNames(data.interactedNpcNames);
        }

        // Force NPCs in the active scene to resync their local hasInteracted flags.
        // This is needed because NPCControllers may have already run Start() before
        // we restored ProgressManager's interacted set.
        var npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        foreach (var npc in npcs)
            npc.SyncFromProgressManager();
    }

    // ---------- Autosave hooks ----------
    private void OnApplicationQuit()
    {
        SaveGameState();
    }
}