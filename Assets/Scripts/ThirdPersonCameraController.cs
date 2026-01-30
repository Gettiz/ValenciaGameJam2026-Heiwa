using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.7f, 0f);
    [SerializeField, Min(0.5f)] private float followDistance = 4.5f;

    [Header("Sensitivity")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField, Min(0f)] private float mouseSensitivity = 0.12f;
    [SerializeField, Min(0f)] private float controllerSensitivity = 180f;
    [SerializeField] private bool invertY;

    [Header("Smoothing")]
    [SerializeField, Min(0.001f)] private float positionSmoothTime = 0.1f;
    [SerializeField, Min(0.001f)] private float rotationSlerpSpeed = 9f;

    [Header("Pitch Limits")]
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;

    private Vector3 positionVelocity;
    private float targetYaw;
    private float targetPitch;

    private void Awake()
    {
        Vector3 euler = transform.eulerAngles;
        targetYaw = euler.y;
        targetPitch = euler.x;
    }

    private void OnEnable()
    {
        EnableLookAction();
    }

    private void OnDisable()
    {
        DisableLookAction();
    }

    private void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        UpdateAngles();
        ApplyCameraTransform();
    }

    private void UpdateAngles()
    {
        if (lookAction == null)
        {
            return;
        }

        InputAction action = lookAction.action;
        if (action == null)
        {
            return;
        }

        Vector2 lookDelta = action.ReadValue<Vector2>();
        InputDevice device = action.activeControl != null ? action.activeControl.device : null;
        float yawInput;
        float pitchInput;

        if (device is Mouse)
        {
            yawInput = lookDelta.x * mouseSensitivity;
            pitchInput = lookDelta.y * mouseSensitivity;
        }
        else
        {
            yawInput = lookDelta.x * controllerSensitivity * Time.deltaTime;
            pitchInput = lookDelta.y * controllerSensitivity * Time.deltaTime;
        }

        targetYaw += yawInput;
        targetPitch += (invertY ? pitchInput : -pitchInput);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
    }

    private void ApplyCameraTransform()
    {
        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
        float lerpFactor = 1f - Mathf.Exp(-rotationSlerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpFactor);

        Vector3 desiredPosition = target.position + targetOffset - transform.rotation * Vector3.forward * followDistance;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);
    }

    private void EnableLookAction()
    {
        if (lookAction == null)
        {
            return;
        }

        InputAction action = lookAction.action;
        if (action == null)
        {
            return;
        }

        action.Enable();
    }

    private void DisableLookAction()
    {
        if (lookAction == null)
        {
            return;
        }

        InputAction action = lookAction.action;
        if (action == null)
        {
            return;
        }

        action.Disable();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (!target)
        {
            return;
        }

        Vector3 lookDirection = target.forward;
        if (lookDirection.sqrMagnitude < 0.001f)
        {
            lookDirection = Vector3.forward;
        }

        targetYaw = Quaternion.LookRotation(lookDirection).eulerAngles.y;
    }
}
