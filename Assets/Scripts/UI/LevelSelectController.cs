using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private string[] levelSceneNames;
    [SerializeField] private int maxUnlockedLevel = 1;

    [Header("Locked Message")]
    [SerializeField] private GameObject lockedMessagePanel;
    [SerializeField] private float lockedMessageDuration = 2f;

    private Coroutine lockedRoutine;

    private void OnEnable()
    {
        maxUnlockedLevel = SaveSystem.GetMaxUnlockedLevel(maxUnlockedLevel);
        if (lockedMessagePanel != null)
        {
            lockedMessagePanel.SetActive(false);
        }
    }

    public void SelectLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelSceneNames.Length)
        {
            return;
        }

        if (levelIndex >= maxUnlockedLevel)
        {
            ShowLockedMessage();
            return;
        }

        string sceneName = levelSceneNames[levelIndex];
        SaveSystem.SaveProgress(sceneName, maxUnlockedLevel);
        SceneManager.LoadScene(sceneName);
    }

    private void ShowLockedMessage()
    {
        if (lockedMessagePanel == null)
        {
            return;
        }

        lockedMessagePanel.SetActive(true);
        if (lockedRoutine != null)
        {
            StopCoroutine(lockedRoutine);
        }
        lockedRoutine = StartCoroutine(HideLockedMessageAfterDelay());
    }

    private IEnumerator HideLockedMessageAfterDelay()
    {
        yield return new WaitForSeconds(lockedMessageDuration);
        if (lockedMessagePanel != null)
        {
            lockedMessagePanel.SetActive(false);
        }
    }
}
