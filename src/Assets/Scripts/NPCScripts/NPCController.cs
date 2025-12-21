using UnityEngine;

public class NPCController : MonoBehaviour, INPC
{
    [SerializeField]
    private string npcName = "NPC";
    
    [SerializeField]
    private string[] dialogLines = new string[]
    {
        "Hello, traveler!",
        "Welcome to our village.",
        "Is there anything I can help you with?"
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
        // Display dialog or trigger NPC behavior
        if (dialogLines.Length > 0 && DialogManager.Instance != null)
        {
            Debug.Log($"{npcName}: {dialogLines[currentDialogIndex]}");

            FacePlayer();

            DialogManager.Instance.StartDialog(npcName, dialogLines);
        }
        
        // Add your dialog system or NPC interaction logic here
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