using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager Instance { get; set; }

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

    private GameObject currentPlayer;

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
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        currentPlayer = Instantiate(characters[PlayerPrefs.GetInt("SpawnInd")], position, Quaternion.identity);
        SetupNPCPlayerReferences();
        DontDestroyOnLoad(currentPlayer);
    }
    
    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPlayerPosition(scene.name);
    }

    private void SetPlayerPosition(string nextSceneName)
    {
        Vector2 newPosition = nextSceneName switch
        {
            "Act1" => act1PlayerPosition,
            "Act2" => act2PlayerPosition,
            "Act3" => act3PlayerPosition,
            "Act4" => act4PlayerPosition,
            _ => position
        };

        if (!currentPlayer) return;
        currentPlayer.transform.position = newPosition;
        SetupNPCPlayerReferences();
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
