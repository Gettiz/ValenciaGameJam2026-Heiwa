using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private AudioClip pickupSfx;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthPlayer playerHealth = other.GetComponentInParent<HealthPlayer>();
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.Heal(healAmount);

        if (pickupSfx != null)
        {
            AudioManager.PlaySfxStatic(pickupSfx);
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
