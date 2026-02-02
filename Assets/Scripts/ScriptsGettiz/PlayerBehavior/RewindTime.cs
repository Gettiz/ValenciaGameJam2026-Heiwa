using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RewindTime : MonoBehaviour
{
    [Header("Settings")]
    public float recordDuration = 5f;
    [SerializeField] public bool isReversing = false;
    
    private List<ObjectState> states = new List<ObjectState>();
    private Rigidbody rb;
    
    private Vector3 lastPosition;
    
    [Header("Materials")]
    private Renderer rendererComponent;
    public Material materialDefault;
    public Material materialRewind;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rendererComponent = GetComponent<Renderer>();
        if (rendererComponent != null && materialDefault != null)
        {
            rendererComponent.sharedMaterial = materialDefault;
        }
    }

    void Update()
    {
        if (isReversing)
        {
            if (rendererComponent.sharedMaterial != materialRewind)
            {
                rendererComponent.sharedMaterial = materialRewind;
            }
            
            ReverseTime();
        }
        else
        {
            if (rendererComponent.sharedMaterial != materialDefault)
            {
                rendererComponent.sharedMaterial = materialDefault;
            }
            
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved > 0.001f || rb.linearVelocity.magnitude > 0.1f)
            {
                RecordState();
            }
        }
        
        lastPosition = transform.position;
    }

    private void ReverseTime()
    {
        if (states.Count > 0)
        {
            ObjectState state = states[0];
            transform.position = state.position;
            transform.rotation = state.rotation;
            rb.linearVelocity = -state.linearVelocity;
            rb.angularVelocity = -state.angularVelocity;
            
            states.RemoveAt(0);
        }
        else
        {
            isReversing = false;
        }
    }

    private void RecordState()
    {
        if (states.Count > Mathf.Round(recordDuration / Time.fixedDeltaTime))
        {
            states.RemoveAt(states.Count - 1);
        }
        
        states.Insert(0, new ObjectState(transform.position, transform.rotation, Vector3.zero, Vector3.one));
    }
}

[System.Serializable]
public struct ObjectState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 linearVelocity;
    public Vector3 angularVelocity;

    public ObjectState(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
    {
        position = pos;
        rotation = rot;
        linearVelocity = vel;
        angularVelocity = angVel;
    }
}