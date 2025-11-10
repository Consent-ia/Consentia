using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    [SerializeField]
    private float speed = 5.0f;

    private Rigidbody2D rigidBody;
    private Animator animator;

    Vector2 movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        //animator.SetFloat("Horizontal", movement.x);
        //animator.SetFloat("Vertical", movement.y);
        //animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + speed * Time.fixedDeltaTime * movement.normalized);
    }
}
