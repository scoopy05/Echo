using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public InputActionReference moveAction;

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;  // NEW: How fast the character turns
    public float gravity = -15f;   // NEW: Gravity force (a bit heavier than real life feels better in games)

    private float verticalVelocity;  // NEW: Tracks our falling speed

    void Update()
    {
        // 1. Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = new Vector3(input.x, 0f, input.y);

        // 2. Apply Rotation
        // Only try to turn if we are actually pressing a movement key
        if (direction.magnitude >= 0.1f)
        {
            // Figure out which way we want to face
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Smoothly rotate the player from their current rotation towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 3. Apply Gravity
        if (controller.isGrounded)
        {
            // If on the ground, stop falling. 
            // We use a small negative number (-2) instead of 0 to firmly stick to the ground over bumps.
            verticalVelocity = -2f; 
        }
        else
        {
            // If in the air, accelerate downwards
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 4. Combine horizontal movement and vertical gravity
        Vector3 finalMovement = new Vector3(direction.x * moveSpeed, verticalVelocity, direction.z * moveSpeed);
        
        // 5. Move the Character Controller
        controller.Move(finalMovement * Time.deltaTime);
    }
}
