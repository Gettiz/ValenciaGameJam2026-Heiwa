using System;
using UnityEngine;

public class PauseBehavior : MonoBehaviour
{
    public GameObject canvasHUD;
    public GameObject canvasPaused;
    public static bool isPaused { get; private set; } = false;

    private void Start()
    {
        Cursor.visible = isPaused;
        canvasHUD.SetActive(true);
        canvasPaused.SetActive(false);
    }
    
    public void SwitchPause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
        
        canvasHUD.SetActive(!isPaused);
        canvasPaused.SetActive(isPaused);
        
        Cursor.visible = isPaused;
        if (isPaused) Cursor.lockState = CursorLockMode.None;
    }
}