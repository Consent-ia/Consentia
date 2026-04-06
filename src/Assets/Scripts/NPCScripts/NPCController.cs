using UnityEngine;

public class NPCController : MonoBehaviour, INPC
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    [SerializeField]
    private NPCDialog npcDialog;

    private Animator animator;
    private Transform playerTransform;

    private bool hasInteracted;
    [SerializeField]
    private bool isQuestionNPC;
    
    private Transform exclamationTransform;

    private void Start()
    {
        animator = GetComponent<Animator>();
        exclamationTransform = transform.Find("Exclamation");

        // Initial sync based on current ProgressManager state (if any).
        SyncFromProgressManager();
    }

    public void SyncFromProgressManager()
    {
        // NPCController also keeps its own hasInteracted flag, so we must sync it.
        if (!ProgressManager.Instance || !npcDialog) return;
        var interacted = ProgressManager.Instance.GetInteractedNpcNames();
        hasInteracted = interacted != null && interacted.Contains(npcDialog.NPCName);
        UpdateExclamationState(!hasInteracted);
    }

    public void Interact()
    {
        if (npcDialog.DialogLines.Length <= 0 || !DialogManager.Instance)
            return;
        Debug.Log($"{npcDialog.NPCName}: Starting dialog");

        FacePlayer();

        DialogManager.Instance.StartDialog(npcDialog.NPCName, npcDialog.DialogLines, hasInteracted);

        if (!isQuestionNPC)
        {
            hasInteracted = true;
        }

        UpdateExclamationState(false);
    }

    private void UpdateExclamationState(bool npcState)
    {
        // Exclamation is optional.
        if (!exclamationTransform) return;
        exclamationTransform.gameObject.SetActive(npcState);
    }

    private void FacePlayer()
    {
        if (!playerTransform || !animator)
            return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetFloat(Horizontal, direction.x);
            animator.SetFloat(Vertical, 0);
        }
        else
        {
            animator.SetFloat(Horizontal, 0);
            animator.SetFloat(Vertical, direction.y);
        }
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}