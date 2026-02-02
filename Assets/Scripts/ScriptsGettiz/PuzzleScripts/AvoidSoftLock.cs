using UnityEngine;

public class AvoidSoftLock : MonoBehaviour
{
    public DoorObject[] doors;
    public float waitTime = 10f;
    private bool playerInside = false;
    private float softLockTimer = 0f;

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
            softLockTimer = 0f;
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
                softLockTimer += Time.deltaTime;
                
                if (softLockTimer >= waitTime)
                {
                    foreach (DoorObject door in doors)
                    {
                        door.OpenDoor();
                    }

                    softLockTimer = 0f;
                }
            }
            else
            {
                softLockTimer = 0f;
            }
        }
    }
}