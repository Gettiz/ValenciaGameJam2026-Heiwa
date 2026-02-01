using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);

    [Header("Look")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float lookSensitivity = 1.2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private bool invertY;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private float rotationSmooth = 12f;

    private InputAction lookAction;
    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (playerInput != null)
        {
            lookAction = playerInput.actions["Look"];
        }

        Vector3 angles = transform.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector2 lookInput = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        float lookScale = 1f;
        if (lookAction != null && lookAction.activeControl != null && lookAction.activeControl.device is Gamepad)
        {
            lookScale = Time.deltaTime;
        }

        float invert = invertY ? 1f : -1f;
        yaw += lookInput.x * lookSensitivity * lookScale;
        pitch += lookInput.y * lookSensitivity * lookScale * invert;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion desiredRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmooth * Time.deltaTime);

        Vector3 desiredPosition = target.position + (transform.rotation * offset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);
    }
}
