using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMove : MonoBehaviour
{
    [Header("InteractableLayer")]
    public LayerMask InteractableObjectLayer;
    private Transform activePlatform;
    private Vector3 lastPlatformPos;

    [Header("Horizontal Movement")] [SerializeField]
    private float acceleration = 30f;

    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float brake = 30f;
    [SerializeField] private float maxSpeed = 2f;

    [Header("Air Control")] [Range(0, 1)] [SerializeField]
    private float airControl = 0.8f;
    [SerializeField] private float maxSpeedInAir = 0.7f;
    private float maxGlobalSpeed;

    [Header("Jump Settings")] [SerializeField]
    private float jumpImpulse = 8f;

    [SerializeField] private float jumpHoldForce = 25f;
    [SerializeField] private float maxJumpHoldTime = 0.25f;

    [Header("Fall Physics")] [SerializeField]
    private float fallMultiplier = 25f;

    [SerializeField] private float lowJumpMultiplier = 15f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    [SerializeField] private PlayerInput playerInput;

    private float moveInput;
    private float jumpTimer;
    private bool isJumping;
    private bool jumpButtonHold;
    private Vector3 groundNormal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();
        
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        if (!PauseBehavior.isPaused)
        {
            bool grounded = IsPlayerOnGround();
        
            if (grounded && activePlatform != null)
            {
                Vector3 platformDelta = activePlatform.position - lastPlatformPos;

                if (platformDelta.magnitude > 0)
                {
                    rb.position += platformDelta;
                }
            
                lastPlatformPos = activePlatform.position;
            }

            MovementBehavior(grounded);
            JumpBehavior(grounded);
            FallSpeed(grounded);
        }
    }

    public void MovementBehavior(bool isGrounded)
    {
        Vector3 moveDir = new Vector3(moveInput, 0, 0);
        Vector3 moveOnPlane = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;
        bool isMoveInputActive = Mathf.Abs(moveInput) > 0.05f;

        float currentAccel;

        if (isGrounded)
            currentAccel = acceleration;
        else
            currentAccel = acceleration * airControl;

        if (isMoveInputActive && isGrounded)
        {
            float alignment = Vector3.Dot(moveOnPlane, rb.linearVelocity.normalized);

            if (alignment >= 0 || rb.linearVelocity.magnitude < 0.1f)
            {
                Acceleration(currentAccel, maxSpeed, moveOnPlane);
            }
            else
            {
                Brake(moveOnPlane, brake);
            }
        }
        else if (isGrounded)
        {
            Deceleration(deceleration);
        }

        if (!isGrounded && isMoveInputActive)
        {
            if (rb.linearVelocity.magnitude >= maxSpeedInAir)
            {
                maxGlobalSpeed = maxSpeedInAir;
            }

            Vector3 finalAirControl = moveOnPlane * (acceleration * airControl);
            Vector3 accelerationForce = finalAirControl * maxGlobalSpeed;
            ApplyForce(accelerationForce);
        }
    }

    private void Acceleration(float accelerationSpeed, float maxGroundSpeed, Vector3 moveDirection)
    {
        maxGlobalSpeed = Mathf.MoveTowards(maxGlobalSpeed, maxGroundSpeed, accelerationSpeed * Time.fixedDeltaTime);

        if (rb.linearVelocity.magnitude >= maxGlobalSpeed)
        {
            maxGlobalSpeed = maxGroundSpeed;
        }

        Vector3 accelerationForce = moveDirection * maxGlobalSpeed * 5f;
        ApplyForce(accelerationForce);
    }

    private void Brake(Vector3 moveDirection, float brakeSpeed)
    {
        maxGlobalSpeed = Mathf.MoveTowards(maxGlobalSpeed, 0, brakeSpeed * Time.fixedDeltaTime);

        Vector3 brakeForce = moveDirection * brakeSpeed;
        ApplyForce(brakeForce);
    }

    private void Deceleration(float decelerationSpeed)
    {
        maxGlobalSpeed = Mathf.MoveTowards(maxGlobalSpeed, 0, decelerationSpeed * Time.fixedDeltaTime);

        Vector3 stopForce = new Vector3(-rb.linearVelocity.x * decelerationSpeed, 0, 0);
        ApplyForce(stopForce);
    }

    private void JumpBehavior(bool grounded)
    {
        if (isJumping && jumpButtonHold && jumpTimer < maxJumpHoldTime)
        {
            ApplyForce(Vector3.up * jumpHoldForce);
            jumpTimer += Time.fixedDeltaTime;
        }

        if (grounded) isJumping = false;
    }

    private void FallSpeed(bool grounded)
    {
        if (grounded) return;

        if (rb.linearVelocity.y < 0)
        {
            ApplyForce(Vector3.down * fallMultiplier);
        }
        else if (rb.linearVelocity.y > 0 && !jumpButtonHold)
        {
            ApplyForce(Vector3.down * lowJumpMultiplier);
        }
    }
    
    

    public bool IsPlayerOnGround()
    {
        float rayLength = (playerCollider.height * 0.5f) + 0.15f;
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, ~0, QueryTriggerInteraction.Ignore))
        {
            groundNormal = hit.normal;
            
            if (((1 << hit.collider.gameObject.layer) & InteractableObjectLayer) != 0)
            {
                if (activePlatform != hit.transform)
                {
                    activePlatform = hit.transform;
                    lastPlatformPos = activePlatform.position;
                }
            }
            else
            {
                activePlatform = null;
            }

            return true;
        }

        activePlatform = null;
        groundNormal = Vector3.up;
        return false;
    }

    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(new Vector3(force.x, force.y, 0), ForceMode.Force);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!PauseBehavior.isPaused)
        {
            if (context.started && IsPlayerOnGround())
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
                rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);

                isJumping = true;
                jumpTimer = 0;
                jumpButtonHold = true;
            }

            if (context.canceled)
            {
                jumpButtonHold = false;
            }
        }
    }
}