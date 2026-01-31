using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public DoorObject[] doorObject;
    private List<Collider> occupants = new List<Collider>(); 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("BoxInteract"))
        {
            if (!occupants.Contains(other))
            {
                occupants.Add(other);
                
                if (occupants.Count == 1)
                {
                    foreach(DoorObject door in doorObject) { door.OpenDoor(); }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (occupants.Contains(other))
        {
            occupants.Remove(other);
            CheckDoorState();
        }
    }

    private void Update()
    {
        //TempFix to update the triggers if boxes are no longer above them
        
        int initialCount = occupants.Count;
        occupants.RemoveAll(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy);

        if (occupants.Count != initialCount)
        {
            CheckDoorState();
        }
    }

    private void CheckDoorState()
    {
        if (occupants.Count == 0)
        {
            foreach(DoorObject door in doorObject) { door.CloseDoor(); }
        }
    }
}
