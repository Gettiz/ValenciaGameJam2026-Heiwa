using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("InteractableLayer")]
    public LayerMask InteractableObjectLayer;
    private Transform activePlatform;
    private Vector3 lastPlatformPos;

    [Header("Horizontal Movement")] [SerializeField]
    private float acceleration = 30f;

    [SerializeField] private float deceleration = 5f;
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

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private PlayerInput playerInput;

    private float rawInput;
    private float jumpTimer;
    private bool isJumping;
    private bool jumpButtonHold;
    private Vector3 groundNormal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        rawInput = playerInput.actions["Move"].ReadValue<Vector2>().x;
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
        Vector3 moveDir = new Vector3(rawInput, 0, 0);
        Vector3 moveOnPlane = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;
        bool isMoveInputActive = Mathf.Abs(rawInput) > 0.05f;

        float currentMaxSpeed = isGrounded ? maxSpeed : maxSpeedInAir;
        float currentAccel = isGrounded ? acceleration : acceleration * airControl;

        if (isMoveInputActive)
        {
            float currentSpeedInDir = Vector3.Dot(rb.linearVelocity, moveOnPlane);
            
            if (currentSpeedInDir < currentMaxSpeed)
            {
                float forceMagnitude = currentAccel * rb.mass;
                ApplyForce(moveOnPlane * forceMagnitude);
            }
            
            if (currentSpeedInDir > currentMaxSpeed)
            {
                Vector3 vel = rb.linearVelocity;
                float verticalY = vel.y;
                vel = Vector3.ClampMagnitude(new Vector3(vel.x, 0, 0), currentMaxSpeed);
                vel.y = verticalY;
                rb.linearVelocity = vel;
            }
        }
        else if (isGrounded)
        {
            Deceleration(deceleration);
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

    private void Deceleration(float decelerationSpeed)
    {
        maxGlobalSpeed = Mathf.MoveTowards(maxGlobalSpeed, 0, decelerationSpeed * Time.fixedDeltaTime);

        Vector3 stopForce = new Vector3(-rb.linearVelocity.x * decelerationSpeed, 0, 0);
        ApplyForce(stopForce);
    }

    private void JumpBehavior(bool grounded)
    {
        if (isJumping)
        {
            if (jumpButtonHold && jumpTimer < maxJumpHoldTime)
            {
                Debug.Log("timerjumpexecuting");
                rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Acceleration);
                jumpTimer += Time.fixedDeltaTime;
            }
            else
            {
                rb.AddForce(Vector3.down * lowJumpMultiplier, ForceMode.Acceleration);
            }
            
            if (grounded && rb.linearVelocity.y <= 0.1f && jumpTimer > 0.1f)
            {
                isJumping = false;
            }
        }
    }

    private void FallSpeed(bool grounded)
    {
        if (grounded) return;
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y > 0 && !jumpButtonHold)
        {
            rb.AddForce(Vector3.down * lowJumpMultiplier, ForceMode.Acceleration);
        }
    }
    
    

    public bool IsPlayerOnGround()
    {
        float rayLength = (playerCollider.height * 0.5f) + 0.15f;
        float sideOffset = playerCollider.radius * 0.9f;
        
        Vector3[] rayPositions = new Vector3[3];
        rayPositions[0] = transform.position;
        rayPositions[1] = transform.position + Vector3.left * sideOffset;
        rayPositions[2] = transform.position + Vector3.right * sideOffset;

        RaycastHit hit;
        bool grounded = false;

        foreach (Vector3 pos in rayPositions)
        {
            Debug.DrawRay(pos, Vector3.down * rayLength, Color.red);

            if (Physics.Raycast(pos, Vector3.down, out hit, rayLength, ~0, QueryTriggerInteraction.Ignore))
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

                grounded = true;
                break;
            }
        }

        if (grounded)
        {
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