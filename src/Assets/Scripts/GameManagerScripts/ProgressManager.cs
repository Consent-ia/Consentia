using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    private static readonly int Activate = Animator.StringToHash(portalTriggerName);
    public static ProgressManager Instance { get; private set; }

    private int npcsCurrentAct;
    private string currentAct;
    private readonly HashSet<string> interactedNPCs = new();
    private bool isActCompleted;
    private GameObject exclamationArcReactor;
    private GameObject portal;
    
    private PlayerInput playerInput;

    [Header("Cutscene Timings")]
    [SerializeField] private float panToPortalSeconds = 1.2f;
    [SerializeField] private float holdOnPortalSeconds = 0.4f;
    [SerializeField] private float panBackSeconds = 1.0f;
    [SerializeField] private float waitAfterActivateSeconds = 1.0f;

    private Animator portalAnimator;
    private const string portalTriggerName = "Activate";

    // Cinemachine cameras
    private CinemachineCamera camPlayer;
    private GameObject camPortal;
    
    private bool cutsceneRunning;
    
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
    
    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentAct = scene.name;
        interactedNPCs.Clear();
        isActCompleted = false;
        
        npcsCurrentAct = FindObjectsByType<NPCController>(FindObjectsSortMode.None).Length;

        DeactivateExclamationArcReactor();
        DeactivatePortal();
        DeactivatePortalCamera();
    }

    public void Initialise(GameObject player)
    {
        if (!player) return;
        playerInput = player.GetComponent<PlayerInput>();
        camPlayer = player.GetComponentInChildren<CinemachineCamera>();
    }

    private void SetPlayerControlEnabled(bool isEnabled)
    {
        // Also disable the Input System source.
        if (!playerInput) return;
        if (isEnabled)
        {
            if (playerInput.actions)
                playerInput.actions.Enable();
            playerInput.ActivateInput();
        }
        else
        {
            playerInput.DeactivateInput();
            if (playerInput.actions)
                playerInput.actions.Disable();
        }
    }

    private void DeactivateExclamationArcReactor()
    {
        if (currentAct != "Act1" && currentAct != "Act5")
            return;
        
        exclamationArcReactor = GameObject.FindGameObjectWithTag("EA");
        if (exclamationArcReactor)
        {
            exclamationArcReactor.SetActive(false);
        }
    }

    private void DeactivatePortal()
    {
        portal = GameObject.FindGameObjectWithTag("Portal");
        if (!portal) return;
        portalAnimator = portal.GetComponent<Animator>();
        portal.SetActive(false);
    }
    
    private void DeactivatePortalCamera()
    {
        camPortal = GameObject.FindGameObjectWithTag("CamPortal");
        if (!camPortal) return;
        camPortal.SetActive(false);
    }

    private void Update()
    {
        if (isActCompleted || interactedNPCs.Count != npcsCurrentAct || currentAct == "IntroAct" || currentAct == "Act6")
            return;
        
        isActCompleted = true;
        if (currentAct is "Act1" or "Act5")
        {
            exclamationArcReactor.SetActive(true);
        }
        else
        {
            PanCameraToPortal();
        }
    }

    public void PanCameraToPortal()
    {
        if (cutsceneRunning)
            return;

        SetPlayerControlEnabled(false);
        
        StartCoroutine(PanCutsceneCoroutine());
    }
    
    private IEnumerator PanCutsceneCoroutine()
    {
        cutsceneRunning = true;
        
        // Disable player input during cutscene
        
        // 2) Switch to portal camera
        camPlayer.enabled = false;
        camPortal.SetActive(true);
        
        // 3) Wait for blend to finish (roughly equals blend time)
        yield return new WaitForSeconds(panToPortalSeconds);
        
        // Optional hold
        yield return new WaitForSeconds(holdOnPortalSeconds);

        portal.SetActive(true);

        // 4) Trigger animation
        if (portalAnimator && !string.IsNullOrEmpty(portalTriggerName))
            portalAnimator.SetTrigger(Activate);

        // Wait for animation (either fixed seconds, or better: Animation Event / StateMachineBehaviour)
        yield return new WaitForSeconds(waitAfterActivateSeconds);

        // 5) Pan back to player: restore player camera priority
        camPlayer.enabled = true;
        camPortal.SetActive(false);
        
        yield return new WaitForSeconds(panBackSeconds);

        // 6) Re-enable input only after everything is done
        SetPlayerControlEnabled(true);

        cutsceneRunning = false;
    }

    public void AddNpc(string npcName)
    {
        if (npcName == "Arc Reactor") return;
        if (!interactedNPCs.Add(npcName))
        {
            Debug.Log($"Already interacted with {npcName}. Ignoring duplicate interaction.");
            return;
        }
        Debug.Log($"Interacted with {interactedNPCs.Count} NPCs in {currentAct}");
    }

    /// <summary>
    /// Returns a copy of interacted NPC names for saving.
    /// </summary>
    public List<string> GetInteractedNpcNames()
    {
        return new List<string>(interactedNPCs);
    }

    /// <summary>
    /// Restores interacted NPC names from save.
    /// </summary>
    public void SetInteractedNpcNames(List<string> npcNames)
    {
        interactedNPCs.Clear();
        if (npcNames == null) return;
        foreach (var npcName in npcNames.Where(npcName => !string.IsNullOrEmpty(npcName)))
        {
            interactedNPCs.Add(npcName);
        }
    }
}
