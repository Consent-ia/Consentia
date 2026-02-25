using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

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

    private GameObject currentPlayer;

    private void Awake()
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
    }

    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        currentPlayer = Instantiate(characters[PlayerPrefs.GetInt("SpawnInd")], position, Quaternion.identity);
        DontDestroyOnLoad(currentPlayer);
    }

    public void SetPlayerPosition(string nextSceneName)
    {
        Vector2 newPosition = nextSceneName switch
        {
            "Act1" => act1PlayerPosition,
            "Act2" => act2PlayerPosition,
            _ => position
        };
        if (currentPlayer != null)
        {
            currentPlayer.transform.position = newPosition;
        }
    }
}
