using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpObject : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask grabLayer;
    
    private List<GameObject> insideTrigger = new List<GameObject>();
    private GameObject grabbedObject = null;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & grabLayer) != 0)
        {
            if (!insideTrigger.Contains(other.gameObject)) 
                insideTrigger.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (insideTrigger.Contains(other.gameObject)) 
            insideTrigger.Remove(other.gameObject);
    }

    public void ToggleGrab(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (grabbedObject == null)
        {
            if (insideTrigger.Count > 0)
            {
                grabbedObject = insideTrigger[0];

                if (grabbedObject.TryGetComponent(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                }

                if (grabbedObject.TryGetComponent(out Collider col))
                {
                    col.enabled = false;
                }

                grabbedObject.transform.SetParent(transform);
                grabbedObject.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (grabbedObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }
            
            if (grabbedObject.TryGetComponent(out Collider col))
            {
                col.enabled = true;
            }

            grabbedObject.transform.SetParent(null);
            grabbedObject = null;
        }
    }
}

