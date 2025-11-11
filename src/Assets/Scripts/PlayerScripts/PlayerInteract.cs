using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    [Range(0.5f, 5f)]
    private float interactRange = 2f;

    [SerializeField]
    private LayerMask npcLayer;

    private Animator animator;
    private Vector2 lastMovementDirection = Vector2.down;
    private PlayerMovement playerMovement;
    private bool isInteractingWithNPC = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact pressed");

            // If already interacting with an NPC, advance dialog
            if (isInteractingWithNPC && DialogManager.Instance != null && DialogManager.Instance.IsDialogActive())
            {
                DialogManager.Instance.DisplayNextLine();
                return;
            }

            // Otherwise, try to interact with a new NPC
            TryInteractWithNPC();
        }
    }

    void Update()
    {
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            lastMovementDirection = new Vector2(horizontal, vertical).normalized;
        }

        // Check if dialog has ended
        if (isInteractingWithNPC && DialogManager.Instance != null && !DialogManager.Instance.IsDialogActive())
        {
            EndInteraction();
        }
    }

    private void TryInteractWithNPC()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = lastMovementDirection;

        Debug.DrawRay(rayOrigin, rayDirection * interactRange, Color.green, 1f);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, interactRange, npcLayer);

        if (hit.collider != null)
        {
            INPC npc = hit.collider.GetComponent<INPC>();
            if (npc != null)
            {
                npc.Interact();
                StartInteraction();
                Debug.Log($"Interacting with {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("No NPC in range");
        }
    }

    private void StartInteraction()
    {
        isInteractingWithNPC = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            playerMovement.movement = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
    }

    private void EndInteraction()
    {
        isInteractingWithNPC = false;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3(lastMovementDirection.x, lastMovementDirection.y, 0);
        Gizmos.DrawRay(transform.position, direction * interactRange);
    }
}