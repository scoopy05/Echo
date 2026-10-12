using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public InputActionReference moveAction;
    public InputActionReference sprintAction;
    public InputActionReference crouchAction;
    public Transform cameraTransform;
    public Animator animator;

    [Header("Speeds")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7.5f;
    public float crouchSpeed = 2f;
    public float turnSpeed = 10f;
    public float gravity = -15f;

    [Header("Crouch Collider")]
    public float standingHeight = 2f;
    public float crouchHeight = 1.2f;

    private float verticalVelocity;
    private bool isCrouching;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Crouch toggle (Press C to crouch/stand)
        if (crouchAction != null && crouchAction.action.WasPressedThisFrame())
        {
            SetCrouching(!isCrouching);
        }

        // 2. Read move input
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 3. Sprint check (Hold Shift while moving)
        bool isSprinting = false;
        if (sprintAction != null && sprintAction.action.IsPressed() && input.magnitude > 0.1f)
        {
            if (isCrouching)
            {
                SetCrouching(false);
            }
            isSprinting = true;
        }

        // 4. Determine movement speed
        float currentMoveSpeed = walkSpeed;
        if (isCrouching)
        {
            currentMoveSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentMoveSpeed = sprintSpeed;
        }

        // 5. Flatten camera directions to the ground plane
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 6. Build movement direction relative to camera
        Vector3 direction = (cameraForward * input.y) + (cameraRight * input.x);

        // 7. Rotate character towards movement direction
        if (direction.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 8. Apply gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 9. Move the character controller
        Vector3 finalMovement = new Vector3(direction.x * currentMoveSpeed, verticalVelocity, direction.z * currentMoveSpeed);
        controller.Move(finalMovement * Time.deltaTime);

        // 10. Update Animator parameters
        if (animator != null)
        {
            // Speed: 0 = Idle, 1 = Walk, 2 = Sprint
            float animSpeed = 0f;
            if (input.magnitude > 0.1f)
            {
                animSpeed = isSprinting ? 2f : 1f;
            }

            animator.SetFloat("Speed", animSpeed);
            animator.SetBool("IsCrouching", isCrouching);
        }
    }

    private void SetCrouching(bool crouch)
    {
        isCrouching = crouch;
        controller.height = isCrouching ? crouchHeight : standingHeight;
        controller.center = new Vector3(0f, controller.height / 2f, 0f);
    }
}
