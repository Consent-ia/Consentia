using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapPageImageController : MonoBehaviour
{
    [Serializable]
    public class ActImage
    {
        public string sceneName;      // e.g. "Act1"
        public GameObject actImage;   // e.g. Act1Image GameObject
    }

    [Header("Assign one entry per Act (scene name -> image object)")]
    [SerializeField] private List<ActImage> actImages = new();

    [Header("If true, hides all images when scene name doesn't match any entry")]
    [SerializeField] private bool hideAllIfNoMatch = true;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Important: OnEnable is called when the Map page is activated (clicked tab),
        // so this ensures the correct image is shown immediately.
        RefreshForCurrentScene();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the map page is currently active, update immediately.
        // (If it's inactive, OnEnable will handle it when user opens the Map tab.)
        RefreshForScene(scene.name);
    }

    public void RefreshForCurrentScene()
    {
        RefreshForScene(SceneManager.GetActiveScene().name);
    }

    public void RefreshForScene(string sceneName)
    {
        bool foundMatch = false;

        foreach (var entry in actImages)
        {
            if (!entry.actImage) continue;

            bool shouldShow = string.Equals(entry.sceneName, sceneName, StringComparison.Ordinal);
            entry.actImage.SetActive(shouldShow);

            if (shouldShow) foundMatch = true;
        }

        if (!foundMatch && hideAllIfNoMatch)
        {
            // Already hidden by loop, but leaving this here for clarity/extension.
        }
    }
}