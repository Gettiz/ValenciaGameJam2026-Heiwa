using System.Collections;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    private Vector3 doorPos;
    public float OpenDistance = 3;
    public float OpenCloseTimer = 0.5f;

    //in case any function is called twice or more
    public bool doorIsOpen = false;
    public bool doorIsClose = true;

    private Coroutine DoorOpenCloseCoroutine;

    void Start()
    {
        doorPos = transform.position;
    }

    public void OpenDoor()
    {
        if (!doorIsOpen)
        {
            doorIsOpen = true;
            doorIsClose = false;
            
            doorPos += new Vector3(0, OpenDistance, 0);

            if (DoorOpenCloseCoroutine != null)
            {
                StopCoroutine(DoorOpenCloseCoroutine);
            }

            DoorOpenCloseCoroutine = StartCoroutine(OpenCloseDoorCoroutine(doorPos));
        }
    }

    public void CloseDoor()
    {
        if (!doorIsClose)
        {
            doorIsOpen = false;
            doorIsClose = true;
            
            doorPos -= new Vector3(0, OpenDistance, 0);

            if (DoorOpenCloseCoroutine != null)
            {
                StopCoroutine(DoorOpenCloseCoroutine);
            }

            DoorOpenCloseCoroutine = StartCoroutine(OpenCloseDoorCoroutine(doorPos));
        }
    }


    IEnumerator OpenCloseDoorCoroutine(Vector3 newDoorPos)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;

        while (elapsedTime < OpenCloseTimer)
        {
            float t = elapsedTime / OpenCloseTimer;
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPos, newDoorPos, t);

            yield return null;
        }

        transform.position = newDoorPos;
        DoorOpenCloseCoroutine = null;
    }
}
