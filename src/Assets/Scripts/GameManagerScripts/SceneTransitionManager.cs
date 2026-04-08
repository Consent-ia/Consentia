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
    
    // IntroAct Buttons
    private GameObject landingScreen;

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
        if (scene.name != "IntroAct") DeactivateLandingScreen();

        player = GameObject.FindGameObjectWithTag("Player");
        playerInput = player ? player.GetComponent<PlayerInput>() : null;
        // NOTE:
        // Do NOT force-enable input on every scene load.
        // In builds, initialization order can differ vs Editor, and this can
        // accidentally override cutscene/dialog/input-lock systems that intentionally
        // disabled input (e.g. when interacting with ArcReactor).
        // Input should be enabled/disabled only by the system that owns the current game state.
    }
    
    private void DeactivateLandingScreen()
    {
        landingScreen = GameObject.FindGameObjectWithTag("LandingScreen");
        if (landingScreen) landingScreen.SetActive(false);
    }

    public void TransitionToScene(string sceneName)
    {
        if (playerInput)
        {
            // Prefer disabling input via the Input System API instead of disabling the component.
            // This is more robust across platforms/builds.
            playerInput.DeactivateInput();
            if (playerInput.actions)
                playerInput.actions.Disable();
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

        // Re-enable input after the transition has visually completed.
        if (!playerInput) yield break;
        if (playerInput.actions)
            playerInput.actions.Enable();
        playerInput.ActivateInput();
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