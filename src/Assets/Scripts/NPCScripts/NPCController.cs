using UnityEngine;

public class NPCController : MonoBehaviour, INPC
{
    [SerializeField]
    private NPCDialog npcDialog;

    private Animator animator;
    private Transform playerTransform;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void Interact()
    {
        if (npcDialog.DialogLines.Length > 0 && DialogManager.Instance != null)
        {
            Debug.Log($"{npcDialog.NPCName}: Starting dialog");

            FacePlayer();

            DialogManager.Instance.StartDialog(npcDialog.NPCName, npcDialog.DialogLines);
        }
    }

    private void FacePlayer()
    {
        if (playerTransform == null || animator == null)
            return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", 0);
        }
        else
        {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", direction.y);
        }
    }
}