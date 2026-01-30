using UnityEngine;

[DisallowMultipleComponent]
public sealed class SimpleEnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 30f;
    [SerializeField] private GameObject deathEffect;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= Mathf.Abs(amount);
        if (currentHealth <= 0f)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (deathEffect)
        {
            GameObject effectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effectInstance, 3f);
        }

        Destroy(gameObject);
    }
}
