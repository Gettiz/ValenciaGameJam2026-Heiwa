using UnityEngine;

public class BubbleTextNextToPlayer : MonoBehaviour
{
    public CanvasGroup targetCanvasGroup;
    public float maxDistance = 5f;
    private Transform playerTransform;
    private bool playerInRange = false;

    private void Start()
    {
        if (targetCanvasGroup != null) targetCanvasGroup.alpha = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            targetCanvasGroup.alpha = 0;
        }
    }

    private void Update()
    {
        if (playerInRange && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            
            float alphaValue = 1 - Mathf.Clamp01(distance / maxDistance);
            
            targetCanvasGroup.alpha = alphaValue;
        }
    }
}

