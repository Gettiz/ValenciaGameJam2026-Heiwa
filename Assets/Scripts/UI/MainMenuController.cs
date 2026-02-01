using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject languageSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject slideshowPanel;

    [Header("Flow")]
    [SerializeField] private SlideshowController slideshowController;
    [SerializeField] private string fallbackContinueScene = "Level1";

    private void Start()
    {
        ShowPanel(menuPanel);
    }

    public void OnStartPressed()
    {
        /*if (SaveSystem.HasSave())
        {
            ShowPanel(levelSelectPanel);
            return;
        }*/

        ShowPanel(slideshowPanel);
        if (slideshowController != null)
        {
            slideshowController.StartSequence();
        }
    }

    public void OnContinuePressed()
    {
        if (!SaveSystem.HasSave())
        {
            OnStartPressed();
            return;
        }

        string scene = SaveSystem.GetLastScene(fallbackContinueScene);
        SceneManager.LoadScene(scene);
    }

    public void OnCreditsPressed()
    {
        ShowPanel(creditsPanel);
    }

    public void OnBackToMenuPressed()
    {
        ShowPanel(menuPanel);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    public void OnSlideShowFinished()
    {
        SceneManager.LoadScene(fallbackContinueScene);
    }

    public void OnLanguagePressed()
    {
        ShowPanel(languageSelectPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        if (menuPanel != null) menuPanel.SetActive(panel == menuPanel);
        if (languageSelectPanel != null) languageSelectPanel.SetActive(panel == languageSelectPanel);
        if (creditsPanel != null) creditsPanel.SetActive(panel == creditsPanel);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(panel == levelSelectPanel);
        if (slideshowPanel != null) slideshowPanel.SetActive(panel == slideshowPanel);
    }
}
