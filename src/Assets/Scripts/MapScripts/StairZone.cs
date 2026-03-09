using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StairZone : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Speed multiplier when on stairs (e.g. 0.7 for slower on stairs)")]
    [Range(0.1f, 2f)]
    private float speedMultiplier = 0.7f;

    private void Awake()
    {
        // Ensure trigger is set
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(speedMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(1f); // Reset to normal speed
        }
    }
}