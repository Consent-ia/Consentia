using UnityEngine;

public class NPCController : MonoBehaviour, INPC
{
    [SerializeField]
    private string npcName = "NPC";
    
    [SerializeField]
    private DialogLine[] dialogLines = new DialogLine[]
    {
        new DialogLine("Hello, traveler!"),
        new DialogLine("Welcome to our village."),
        new DialogLine("Is there anything I can help you with?")
    };
    
    private int currentDialogIndex = 0;
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
        if (dialogLines.Length > 0 && DialogManager.Instance != null)
        {
            Debug.Log($"{npcName}: Starting dialog");

            FacePlayer();

            DialogManager.Instance.StartDialog(npcName, dialogLines);
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