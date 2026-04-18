using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    [SerializeField]
    [Range(0.5f, 5f)]
    private float interactRange = 2f;

    [SerializeField]
    private LayerMask npcLayer;

    private Animator animator;
    private Vector2 lastMovementDirection = Vector2.down;
    private PlayerMovement playerMovement;
    private bool isInteractingWithNPC;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) 
            return;

        // If Privacy Profile is visible, dismiss it
        if (PrivacyProfileScreen.Instance && PrivacyProfileScreen.Instance.IsVisible())
        {
            PrivacyProfileScreen.Instance.Hide();
            return;
        }

        // If waiting for choice selection, confirm the choice
        if (DialogManager.Instance && DialogManager.Instance.IsWaitingForChoice())
        {
            DialogManager.Instance.SelectChoice();
            return;
        }

        // If already interacting with an NPC, advance dialog
        if (isInteractingWithNPC && DialogManager.Instance && DialogManager.Instance.IsDialogActive())
        {
            DialogManager.Instance.DisplayNextLine();
            return;
        }

        // Otherwise, try to interact with a new NPC
        TryInteractWithNPC();
    }

    // Bind this to your Navigate action in the Input Action Asset
    public void Navigate(InputAction.CallbackContext context)
    {
        if (!context.performed || !DialogManager.Instance ||
            !DialogManager.Instance.IsWaitingForChoice())
            return;
        
        Vector2 direction = context.ReadValue<Vector2>();
        DialogManager.Instance.NavigateChoices(direction);
    }

    private void Update()
    {
        float horizontal = animator.GetFloat(Horizontal);
        float vertical = animator.GetFloat(Vertical);

        if (horizontal != 0 || vertical != 0)
        {
            lastMovementDirection = new Vector2(horizontal, vertical).normalized;
        }

        // Check if dialog has ended
        if (isInteractingWithNPC && DialogManager.Instance && !DialogManager.Instance.IsDialogActive())
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

        if (!hit.collider) 
            return;
        INPC npc = hit.collider.GetComponent<INPC>();
        if (npc == null)
            return;
        npc.Interact();
        StartInteraction();
        Debug.Log($"Interacting with {hit.collider.name}");
    }

    private void StartInteraction()
    {
        isInteractingWithNPC = true;

        if (!playerMovement)
            return;
        
        playerMovement.enabled = false;
        playerMovement.movement = Vector2.zero;
        animator.SetFloat(Speed, 0);
    }

    private void EndInteraction()
    {
        isInteractingWithNPC = false;

        if (playerMovement)
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