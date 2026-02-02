using UnityEngine;

public class ForwardPlayerCamera : MonoBehaviour
{
    public Transform target;

    [Header("Lead Settings")] 
    public float leadDistance = 2.0f;
    public float centerDelay = 0.5f;

    [Header("Smoothing")] 
    public float turnSmoothTime = 0.05f;
    public float steadySmoothTime = 0.2f;
    public float centerDelayTimer = 0.5f;

    private Vector3 currentVelocity;
    private Vector3 targetOffset;
    private Vector3 lastMoveDir = Vector3.right;
    private float stopTimer = 0.0f;
    private Rigidbody rb;

    private void Start()
    {
        if (target != null)
        {
            rb = target.GetComponent<Rigidbody>();
        }
    }
    
    private void LateUpdate()
    {
        Vector3 velocity;
        if (rb != null)
            velocity = rb.linearVelocity;
        else
            velocity = Vector3.zero;
        
        float speed = velocity.magnitude;

        if (speed > 0.1f)
        {
            lastMoveDir = velocity.normalized;
            targetOffset = lastMoveDir * leadDistance;
            stopTimer = 0;
        }
        else
        {
            stopTimer += Time.deltaTime;
            if (stopTimer >= centerDelay)
            {
                targetOffset = Vector3.Lerp(targetOffset, Vector3.zero, centerDelayTimer * Time.deltaTime);
            }
        }

        float currentSmoothTime = steadySmoothTime;

        if (speed > 0.1f)
        {
            float dot = Vector3.Dot(velocity.normalized, targetOffset.normalized);
            if (dot < 0.5f)
            {
                currentSmoothTime = turnSmoothTime;
            }
        }
        
        Vector3 desiredPosition = target.position + targetOffset;
        desiredPosition.z = transform.position.z;
        
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, currentSmoothTime);
    }
}