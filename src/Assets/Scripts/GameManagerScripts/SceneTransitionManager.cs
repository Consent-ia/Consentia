using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    [Range(0.1f, 2f)]
    private float fadeOutDuration = 0.9f;

    [SerializeField]
    [Range(0.1f, 2f)]
    private float fadeInDuration = 0.4f;

    private GameObject player;
    private PlayerInput playerInput;

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

    private void Start()
    {
        // Fade in when scene starts
        if (fadeImage)
        {
            StartCoroutine(FadeIn());
        }
    }
    
    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInput = player ? player.GetComponent<PlayerInput>() : null;
        if (playerInput)
        {
            playerInput.enabled = true;
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (playerInput)
        {
            playerInput.enabled = false;
        }
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        // Wait for scene to be ready
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate scene
        asyncLoad.allowSceneActivation = true;

        // Wait one frame for scene to fully initialize
        yield return null;

        // Wait another frame to ensure position is applied
        yield return null;

        // Fade in
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        if (!fadeImage)
            yield break;

        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0.9f;
        fadeImage.color = color;
    }

    private IEnumerator FadeIn()
    {
        if (!fadeImage)
            yield break;

        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 0.9f;
        fadeImage.color = color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 0.9f - Mathf.Clamp01(elapsed / fadeInDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}