using UnityEngine;

public class AvoidSoftLock : MonoBehaviour
{
    public DoorObject[] doors;
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            bool allDoorsAreClosed = true;

            foreach (DoorObject door in doors)
            {
                if (door.doorIsOpen)
                {
                    allDoorsAreClosed = false;
                    break; 
                }
            }
            
            if (allDoorsAreClosed)
            {
                foreach (DoorObject door in doors)
                {
                    door.OpenDoor();
                }
            }
        }
    }
}

