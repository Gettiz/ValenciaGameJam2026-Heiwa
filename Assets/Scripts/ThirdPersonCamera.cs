using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] string lookActionName = "Look";
    [SerializeField] Vector2 sensitivity = new Vector2(90f, 70f);
    [SerializeField] Vector2 pitchLimits = new Vector2(-35f, 65f);
    [SerializeField] float followLerp = 12f;
    [SerializeField] float distance = 6f;
    [SerializeField] bool lockCursorOnStart = true;
    [SerializeField] bool logLookDelta;
    [SerializeField] bool autoSwitchKeyboardScheme = true;

    InputAction _lookAction;
    float _yaw;
    float _pitch;

    void Awake()
    {
        if (!target && playerInput)
            target = playerInput.transform;
    }

    void Start()
    {
        if (!playerInput && target)
            playerInput = target.GetComponent<PlayerInput>();

        if (TryCacheLookAction() && _lookAction != null && !_lookAction.enabled)
            _lookAction.Enable();

        if (target)
        {
            Vector3 offset = transform.position - target.position;
            Quaternion angles = Quaternion.LookRotation(-offset, Vector3.up);
            _yaw = angles.eulerAngles.y;
            _pitch = angles.eulerAngles.x > 180f ? angles.eulerAngles.x - 360f : angles.eulerAngles.x;
        }

        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnEnable()
    {
        if (TryCacheLookAction() && _lookAction != null && !_lookAction.enabled)
            _lookAction.Enable();
    }

    void OnDisable()
    {
        _lookAction?.Disable();
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    bool TryCacheLookAction()
    {
        if (!playerInput)
        {
            Debug.LogWarning("ThirdPersonCamera: sin PlayerInput asignado.", this);
            return false;
        }

        var actions = playerInput.actions;
        if (actions == null)
        {
            Debug.LogWarning("ThirdPersonCamera: PlayerInput sin asset de acciones.", this);
            return false;
        }

        InputAction resolved = actions.FindAction(lookActionName, false);

        if (resolved == null && playerInput.currentActionMap != null)
            resolved = playerInput.currentActionMap.FindAction(lookActionName, false);

        if (resolved == null)
        {
            foreach (var map in actions.actionMaps)
            {
                resolved = map.FindAction(lookActionName, false);
                if (resolved != null)
                {
                    if (playerInput.currentActionMap != map)
                        playerInput.SwitchCurrentActionMap(map.name);
                    break;
                }
            }
        }

        if (resolved == null)
        {
            Debug.LogWarning($"ThirdPersonCamera: no se encontro la accion '{lookActionName}'.", this);
            return false;
        }

        if (_lookAction == resolved) return true;

        if (_lookAction != null && _lookAction.enabled)
            _lookAction.Disable();

        _lookAction = resolved;
        return true;
    }

    void LateUpdate()
    {
        if (!target || _lookAction == null) return;

        Vector2 lookDelta = _lookAction.ReadValue<Vector2>();
        if (logLookDelta)
            Debug.Log($"Look delta: {lookDelta}");

        if (autoSwitchKeyboardScheme && playerInput && lookDelta.sqrMagnitude > 0.000001f)
        {
            const string keyboardScheme = "Keyboard&Mouse";
            if (playerInput.currentControlScheme != keyboardScheme && Keyboard.current != null && Mouse.current != null)
            {
                try
                {
                    playerInput.SwitchCurrentControlScheme(keyboardScheme, Keyboard.current, Mouse.current);
                }
                catch (System.ArgumentException)
                {
                    // esquema ausente, dejar registro una vez
                    Debug.LogWarning("ThirdPersonCamera: control scheme 'Keyboard&Mouse' no definido en el asset.", this);
                }
            }
        }
        _yaw += lookDelta.x * sensitivity.x * Time.deltaTime;
        _pitch -= lookDelta.y * sensitivity.y * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 desiredPosition = target.position - rotation * Vector3.forward * distance;

        float lerpFactor = followLerp <= 0f ? 1f : Mathf.Clamp01(followLerp * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, lerpFactor);
        transform.rotation = rotation;
    }
}