using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public InputActionReference moveAction;
    public Transform cameraTransform;
    public Animator animator;        // NEW: reference to the character's Animator

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float gravity = -15f;

    private float verticalVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 1. Get camera directions, flattened to the ground plane
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 2. Build movement direction relative to camera
        Vector3 direction = (cameraForward * input.y) + (cameraRight * input.x);

        // 3. Rotate character to face movement direction
        if (direction.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 4. Apply gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 5. Move the character
        Vector3 finalMovement = new Vector3(direction.x * moveSpeed, verticalVelocity, direction.z * moveSpeed);
        controller.Move(finalMovement * Time.deltaTime);

        // 6. Tell the Animator how fast we're moving
        float currentSpeed = new Vector2(input.x, input.y).magnitude;
        animator.SetFloat("Speed", currentSpeed);
    }
}
