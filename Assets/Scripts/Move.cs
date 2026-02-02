using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] string moveActionName = "Move";
    [SerializeField] float speed = 6f;
    [SerializeField] float jumpSpeed = 8f;
    [SerializeField] float downGravityBoost = 12f;
    [SerializeField] Transform cameraTransform;
    [SerializeField] bool logMoveInput;
    [SerializeField] bool autoSwitchKeyboardScheme = true;

    PlayerInput playerInput;
    InputAction moveAction;
    Rigidbody rb;
    Vector2 moveVector;
    bool isJumping;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;
        CacheMoveAction();
    }

    void OnEnable()
    {
        CacheMoveAction();
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
    }

    void CacheMoveAction()
    {
        if (!playerInput || playerInput.actions == null) return;
        moveAction = playerInput.actions.FindAction(moveActionName, false);
        if (moveAction == null)
            Debug.LogWarning($"Move: no se encontro la accion '{moveActionName}'.", this);
    }

    void Update()
    {
        if (moveAction == null) return;
        moveVector = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);
        if (logMoveInput)
            Debug.Log($"Move input {moveVector} | Scheme {playerInput?.currentControlScheme}");

        if (autoSwitchKeyboardScheme && playerInput && playerInput.currentControlScheme != "Keyboard&Mouse")
        {
            if (Keyboard.current != null && (Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed ||
                                             Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed ||
                                             Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed ||
                                             Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed))
            {
                try
                {
                    playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogWarning("Move: control scheme 'Keyboard&Mouse' no definido en el asset.", this);
                }
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector3 camForward = Vector3.forward;
        Vector3 camRight = Vector3.right;

        if (cameraTransform)
        {
            camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        }

        Vector3 desiredHorizontal = (camForward * moveVector.y + camRight * moveVector.x) * speed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = desiredHorizontal.x;
        velocity.z = desiredHorizontal.z;
        if (isJumping)
            velocity += Vector3.down * downGravityBoost * Time.fixedDeltaTime;
        rb.linearVelocity = velocity;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !isJumping)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            isJumping = true;
        }

        if (context.canceled)
            isJumping = false;
    }
    
    public void Fly(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
        }
    }
}
