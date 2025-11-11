using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    [SerializeField]
    private float speed = 5.0f;

    private Rigidbody2D rigidBody;
    private Animator animator;

    public Vector2 movement { get; set; }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    void Update()
    {
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + speed * Time.fixedDeltaTime * movement.normalized);
    }
}
