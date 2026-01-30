using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public sealed class PlayerCharacterController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference attackAction;

    [Header("Movement")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField, Min(0.1f)] private float moveSpeed = 6f;
    [SerializeField, Min(0.1f)] private float acceleration = 14f;
    [SerializeField, Min(0.1f)] private float deceleration = 18f;
    [SerializeField, Range(1f, 30f)] private float rotationLerpSpeed = 12f;

    [Header("Jump & Gravity")]
    [SerializeField, Min(0.1f)] private float jumpHeight = 2.2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField, Min(0f)] private float groundedBufferTime = 0.2f;
    [SerializeField] private float groundedCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundedLayers = ~0;

    [Header("Combat")]
    [SerializeField] private WeaponShooter weapon;
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool drawDebugGizmos;

    private CharacterController characterController;
    private Vector3 planarVelocity;
    private float verticalVelocity;
    private float groundedTimer;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int JumpParam = Animator.StringToHash("Jump");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (!playerInput)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (!cameraTransform && Camera.main)
        {
            cameraTransform = Camera.main.transform;
        }

        if (moveAction == null)
        {
            Debug.LogWarning("Move action is not assigned on PlayerCharacterController.", this);
        }
    }

    private void OnEnable()
    {
        EnableAction(moveAction);
        EnableAction(jumpAction);
        EnableAction(fireAction, OnFirePerformed, true);
        EnableAction(attackAction, OnAttackPerformed, true);
    }

    private void OnDisable()
    {
        DisableAction(moveAction);
        DisableAction(jumpAction);
        DisableAction(fireAction, OnFirePerformed, true);
        DisableAction(attackAction, OnAttackPerformed, true);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        UpdateGroundedState(deltaTime);
        HandleMovement(deltaTime);
        HandleRotation(deltaTime);
        HandleGravity(deltaTime);
        ApplyMotion(deltaTime);
    }

    private void UpdateGroundedState(float deltaTime)
    {
        bool isGrounded = CheckGrounded();
        if (isGrounded)
        {
            groundedTimer = groundedBufferTime;
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            groundedTimer -= deltaTime;
        }
    }

    private void HandleMovement(float deltaTime)
    {
        if (moveAction == null || moveAction.action == null)
        {
            planarVelocity = Vector3.MoveTowards(planarVelocity, Vector3.zero, deceleration * deltaTime);
            UpdateAnimatorSpeed(0f);
            return;
        }

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 desiredVelocity = ComputeDesiredVelocity(moveInput);
        float targetSpeed = desiredVelocity.magnitude;
        float accelerationRate = targetSpeed > 0.01f ? acceleration : deceleration;
        planarVelocity = Vector3.MoveTowards(planarVelocity, desiredVelocity, accelerationRate * deltaTime);
        UpdateAnimatorSpeed(planarVelocity.magnitude / moveSpeed);

        if (jumpAction != null && jumpAction.action != null && jumpAction.action.triggered && groundedTimer > 0f)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetTrigger(JumpParam);
        }
    }

    private void HandleRotation(float deltaTime)
    {
        Vector3 planar = planarVelocity;
        planar.y = 0f;
        if (planar.sqrMagnitude < 0.0005f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(planar.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * deltaTime);
    }

    private void HandleGravity(float deltaTime)
    {
        verticalVelocity += gravity * deltaTime;
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
    }

    private void ApplyMotion(float deltaTime)
    {
        Vector3 motion = planarVelocity;
        motion.y = verticalVelocity;
        characterController.Move(motion * deltaTime);
    }

    private Vector3 ComputeDesiredVelocity(Vector2 input)
    {
        Vector3 inputVector = new Vector3(input.x, 0f, input.y);
        if (inputVector.sqrMagnitude > 1f)
        {
            inputVector.Normalize();
        }

        if (!cameraTransform)
        {
            return inputVector * moveSpeed;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Align movement with camera so "forward" input always goes forward relative to the camera.
        Vector3 adjusted = forward * inputVector.z + right * inputVector.x;
        if (adjusted.sqrMagnitude > 1f)
        {
            adjusted.Normalize();
        }

        return adjusted * moveSpeed;
    }

    private bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        bool hit = Physics.SphereCast(origin, groundedCheckRadius, Vector3.down, out RaycastHit info, groundedCheckRadius + 0.2f, groundedLayers, QueryTriggerInteraction.Ignore);
        return hit || characterController.isGrounded;
    }

    private void UpdateAnimatorSpeed(float normalizedSpeed)
    {
        if (!animator)
        {
            return;
        }

        animator.SetFloat(SpeedParam, normalizedSpeed, 0.1f, Time.deltaTime);
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        Vector3 aimDirection = cameraTransform ? cameraTransform.forward : transform.forward;
        weapon?.TryShoot(aimDirection);
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        animator?.SetTrigger(AttackTrigger);
    }

    private static void EnableAction(InputActionReference reference, Action<InputAction.CallbackContext> callback = null, bool useCallbacks = false)
    {
        if (reference == null)
        {
            return;
        }

        InputAction action = reference.action;
        if (action == null)
        {
            return;
        }

        action.Enable();
        if (useCallbacks && callback != null)
        {
            action.performed += callback;
        }
    }

    private static void DisableAction(InputActionReference reference, Action<InputAction.CallbackContext> callback = null, bool useCallbacks = false)
    {
        if (reference == null)
        {
            return;
        }

        InputAction action = reference.action;
        if (action == null)
        {
            return;
        }

        if (useCallbacks && callback != null)
        {
            action.performed -= callback;
        }

        action.Disable();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugGizmos)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.1f, groundedCheckRadius);
    }
}
