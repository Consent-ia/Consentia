using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance { get; set; }

    [SerializeField]
    private GameObject[] persistentObjects;

    private void Awake()
    {
        // Singleton pattern
        if (!Instance)
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
