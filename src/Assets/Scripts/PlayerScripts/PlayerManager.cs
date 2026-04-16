using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager Instance { get; set; }

    private static readonly string[] GameplayScenes = { "Act1", "Act2", "Act3", "Act4", "Act5" };

    // Static cache so load can request a position even before PlayerManager exists.
    private static bool s_hasPendingLoadedPosition;
    private static Vector3 s_pendingLoadedPosition;

    [Header("Player Characters")]
    [SerializeField]
    private GameObject[] characters;

    [Header("Spawn Position")]
    [SerializeField]
    private Vector2 position;

    [Header("Player Positions")]
    [SerializeField]
    private Vector2 act1PlayerPosition;
    [SerializeField]
    private Vector2 act2PlayerPosition;
    [SerializeField]
    private Vector2 act3PlayerPosition;
    [SerializeField]
    private Vector2 act4PlayerPosition;
    [SerializeField]
    private Vector2 act5PlayerPosition;

    private GameObject currentPlayer;

    // If a load system wants to place the player at a specific position, it can
    // request a one-time override for the next sceneLoaded.
    private bool hasPendingLoadedPosition;
    private Vector3 pendingLoadedPosition;

    public static void SetNextLoadedPlayerPosition(Vector3 position)
    {
        // Cache statically so this works even if PlayerManager hasn't Awoken yet.
        s_hasPendingLoadedPosition = true;
        s_pendingLoadedPosition = position;

        if (!Instance) return;
        Instance.hasPendingLoadedPosition = true;
        Instance.pendingLoadedPosition = position;
    }

    private void Awake()
    {
        // Singleton pattern
        if (!Instance)
        {
            Instance = this;

            // Pull any pending loaded position requested before Awake.
            if (!s_hasPendingLoadedPosition) return;
            hasPendingLoadedPosition = true;
            pendingLoadedPosition = s_pendingLoadedPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SpawnPlayer(string sceneName)
    {
        Vector2 newPosition;
        if (hasPendingLoadedPosition)
        {
            hasPendingLoadedPosition = false;
            s_hasPendingLoadedPosition = false;
            newPosition = pendingLoadedPosition;
        }
        else
        {
            newPosition = SetNewPosition(sceneName);
        }
        currentPlayer = Instantiate(characters[PlayerPrefs.GetInt("SpawnInd")], newPosition, Quaternion.identity);
        SetupNPCPlayerReferences();
        DontDestroyOnLoad(currentPlayer);
    }
    
    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure the player exists in every gameplay scene (not only Act1).
        if (!currentPlayer && IsGameplayScene(scene.name))
            SpawnPlayer(scene.name);

        // Keep ProgressManager hooked up whenever a player exists.
        if (currentPlayer && ProgressManager.Instance)
            ProgressManager.Instance.Initialise(currentPlayer);
        
        // Only apply default act spawn positions for normal transitions between acts.
        // If the player doesn't exist (e.g. IntroAct), do nothing.
        if (currentPlayer)
            SetPlayerPosition(scene.name);
    }

    private static bool IsGameplayScene(string sceneName)
    {
        return GameplayScenes.Any(t => t == sceneName);
    }

    private void SetPlayerPosition(string nextSceneName)
    {
        Vector2 newPosition = SetNewPosition(nextSceneName);

        if (!currentPlayer) return;
        currentPlayer.transform.position = newPosition;
        SetupNPCPlayerReferences();
    }

    private Vector2 SetNewPosition(string nextSceneName)
    {
        Vector2 newPosition = nextSceneName switch
        {
            "Act1" => act1PlayerPosition,
            "Act2" => act2PlayerPosition,
            "Act3" => act3PlayerPosition,
            "Act4" => act4PlayerPosition,
            "Act5" => act5PlayerPosition,
            _ => position
        };
        return newPosition;
    }

    private void SetupNPCPlayerReferences()
    {
        NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        foreach (var npc in npcs)
        {
            npc.SetPlayerTransform(currentPlayer.transform);
        }
    }
}
