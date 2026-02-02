using System;
using Unity.VisualScripting;
using UnityEngine;

public class BoxGetCloserToPlayer : MonoBehaviour
{
    [Range(-20, -5)] public float normalDistance;
    [Range(-20, -5)] public float closerDistance;
    public float speed = 5f;
    
    public GameObject target;
    private float currentTargetZ;

    private void Start()
    {
        currentTargetZ = normalDistance;
        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, normalDistance);
    }

    private void Update()
    {
        Vector3 currentPos = target.transform.position;
        float newZ = Mathf.MoveTowards(currentPos.z, currentTargetZ, speed * Time.deltaTime);
        target.transform.position = new Vector3(currentPos.x, currentPos.y, newZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentTargetZ = closerDistance;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentTargetZ = normalDistance;
        }
    }
}

