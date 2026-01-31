using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayToPointer : MonoBehaviour
{
    [Header("Input")]
    private PlayerInput playerInput;

    [Header("Settings")]
    public Transform origin;
    public float maxDistance = 50f; 
    public LayerMask interactableLayer;

    [Header("Visuals")]
    public LineRenderer lineRenderer;

    private Vector3 mouseWorldTarget;
    private RaycastHit hitInfo;
    private bool hitSomething;
    
    private RewindTime currentRewindTarget;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        Vector2 mousePos = playerInput.actions["MousePosition"].ReadValue<Vector2>();
        
        float depth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 screenPosWithDepth = new Vector3(mousePos.x, mousePos.y, depth);
        
        mouseWorldTarget = Camera.main.ScreenToWorldPoint(screenPosWithDepth);
        mouseWorldTarget.z = 0;
        
        CalculateRay();
        UpdateLineRenderer();
    }

    void CalculateRay()
    {
        if (origin == null) return;
        
        Vector3 direction = (mouseWorldTarget - origin.position).normalized;

        if (Physics.Raycast(origin.position, direction, out hitInfo, maxDistance, interactableLayer))
        {
            hitSomething = true;
        }
        else
        {
            hitSomething = false;
        }
    }

    void UpdateLineRenderer()
    {
        if (hitSomething)
        {
            lineRenderer.enabled = true;
            
            lineRenderer.SetPosition(0, origin.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
    
    public void InputRewindToggle(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (hitSomething)
            {
                if (hitInfo.collider.TryGetComponent(out RewindTime rewindComp))
                {
                    currentRewindTarget = rewindComp;
                    currentRewindTarget.isReversing = true;
                }
            }
        }

        if (context.canceled)
        {
            if (currentRewindTarget != null)
            {
                currentRewindTarget.isReversing = false;
                currentRewindTarget = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (origin == null) return;
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(origin.position, mouseWorldTarget);
        
        if (Application.isPlaying && hitSomething)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin.position, hitInfo.point);
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }
    }
}

