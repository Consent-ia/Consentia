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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact pressed");
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
    }

    private void TryInteractWithNPC()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = lastMovementDirection;

        Debug.DrawRay(rayOrigin, rayDirection * interactRange, Color.green, 1f);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, interactRange, npcLayer);

        if (hit.collider != null)
        {
            // Check if the hit object has an NPC component or interface
            INPC npc = hit.collider.GetComponent<INPC>();
            if (npc != null)
            {
                npc.Interact();
                Debug.Log($"Interacting with {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("No NPC in range");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3(lastMovementDirection.x, lastMovementDirection.y, 0);
        Gizmos.DrawRay(transform.position, direction * interactRange);
    }
}