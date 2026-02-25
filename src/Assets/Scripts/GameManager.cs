using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject[] persistentObjects;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            MarkPersistentObjects();
        }
        else
        {
            CleanUpAndDestroy();
        }
    }

    private void MarkPersistentObjects()
    {
        // Mark specified objects as persistent
        foreach (GameObject obj in persistentObjects)
        {
            DontDestroyOnLoad(obj);
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
    }
}
