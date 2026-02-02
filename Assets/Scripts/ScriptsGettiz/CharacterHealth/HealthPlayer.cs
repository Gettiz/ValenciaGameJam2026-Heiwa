using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            currentHealth = 0;
            RestartScene();
        }
        else
        {
            currentHealth = math.clamp(currentHealth, 0, maxHealth);
        }
    }

    public void RestartScene()
    {
        PauseBehavior.isPaused = false;
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Damage(float damageAmount)
    {
        Hit(damageAmount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentHealth = math.clamp(currentHealth + amount, 0, maxHealth);
    }
}