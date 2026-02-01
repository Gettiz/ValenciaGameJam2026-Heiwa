using Unity.Mathematics;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100;
    public float currentHealth = 1;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string hitTrigger = "hit";

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Hit(float damage)
    {
        currentHealth -= damage;
        TriggerHitAnimation();
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            currentHealth = math.clamp(currentHealth, 0, maxHealth);
        }
    }

    public void Damage(float damageAmount)
    {
        Hit(damageAmount);
    }

    private void TriggerHitAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(hitTrigger))
        {
            animator.SetTrigger(hitTrigger);
        }
    }
}
