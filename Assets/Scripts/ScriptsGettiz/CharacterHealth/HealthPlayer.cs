using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class HealthPlayer : MonoBehaviour, IDamageable
{
    public Slider HealthSlider;

    public float maxHealth = 100;
    public float currentHealth = 1;
    
    
    private void Start()
    {
        currentHealth = maxHealth;
        HealthSlider.maxValue = maxHealth;
    }

    private void Update()
    {
        HealthSlider.value = currentHealth;
    }

    public void Hit(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            //TODO CHANGE THIS TO REWIND
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
}