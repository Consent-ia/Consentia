using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Speed = Animator.StringToHash("Speed");

    [Range(0.0f, 100.0f)]
    [SerializeField]
    private float speed = 5.0f;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private float speedMultiplier = 1f; // Modified by StairZone

    public Vector2 movement { get; set; }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (movement != Vector2.zero)
        {
            animator.SetFloat(Horizontal, movement.x);
            animator.SetFloat(Vertical, movement.y);
        }
        animator.SetFloat(Speed, movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + speed * speedMultiplier * Time.fixedDeltaTime * movement.normalized);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}
