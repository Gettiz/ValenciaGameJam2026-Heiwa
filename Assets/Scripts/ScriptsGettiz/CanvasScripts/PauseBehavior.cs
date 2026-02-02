using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseBehavior : MonoBehaviour
{
    public GameObject canvasHUD;
    public GameObject canvasPaused;
    [Header("Pause Panels")]
    [SerializeField] private GameObject pauseMainPanel;
    [SerializeField] private GameObject languagePanel;
    private string mainMenuSceneName = "MainMenu";
#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] private SceneAsset mainMenuScene;
#endif
    public static bool isPaused = false;

    private void Start()
    {
        Cursor.visible = isPaused;
        canvasHUD.SetActive(true);
        canvasPaused.SetActive(false);
        ShowPauseMain();
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

        if (isPaused)
        {
            ShowPauseMain();
        }
        
        Cursor.visible = isPaused;
        if (isPaused) Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ShowLanguageMenu()
    {
        if (pauseMainPanel != null) pauseMainPanel.SetActive(false);
        if (languagePanel != null) languagePanel.SetActive(true);
    }

    public void ShowPauseMain()
    {
        if (pauseMainPanel != null) pauseMainPanel.SetActive(true);
        if (languagePanel != null) languagePanel.SetActive(false);
    }

    public void RestartScene()
    {
        RestartSceneStatic();
    }

    public void QuitGame()
    {
        QuitGameStatic();
    }

    public static void RestartSceneStatic()
    {
        isPaused = false;
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public static void QuitGameStatic()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Application.Quit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (mainMenuScene != null)
        {
            mainMenuSceneName = mainMenuScene.name;
        }
    }
#endif
}