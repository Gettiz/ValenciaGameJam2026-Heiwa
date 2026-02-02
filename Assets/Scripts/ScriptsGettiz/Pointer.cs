using UnityEngine;
using UnityEngine.InputSystem;

public class Pointer : MonoBehaviour
{
    [Header("Input")] private PlayerInput playerInput;

    private Vector3 mouseWorldTarget;
    public GameObject pointerMesh;
    public float zOffset;

    private RewindTime currentRewindTarget;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (!PauseBehavior.isPaused)
        {
            Vector2 mousePos = playerInput.actions["MousePosition"].ReadValue<Vector2>();

            float depth = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 screenPosWithDepth = new Vector3(mousePos.x, mousePos.y, depth);

            mouseWorldTarget = Camera.main.ScreenToWorldPoint(screenPosWithDepth);
            mouseWorldTarget.z = zOffset;

            pointerMesh.transform.position = mouseWorldTarget;
        }
    }
}