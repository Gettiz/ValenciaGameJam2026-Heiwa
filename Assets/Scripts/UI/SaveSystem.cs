using UnityEngine;

public static class SaveSystem
{
    private const string HasSaveKey = "HasSave";
    private const string MaxUnlockedLevelKey = "MaxUnlockedLevel";
    private const string LastSceneKey = "LastScene";

    public static bool HasSave()
    {
        return PlayerPrefs.GetInt(HasSaveKey, 0) == 1;
    }

    public static void SetHasSave(bool value)
    {
        PlayerPrefs.SetInt(HasSaveKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static int GetMaxUnlockedLevel(int defaultValue = 1)
    {
        return PlayerPrefs.GetInt(MaxUnlockedLevelKey, defaultValue);
    }

    public static void SetMaxUnlockedLevel(int level)
    {
        PlayerPrefs.SetInt(MaxUnlockedLevelKey, Mathf.Max(1, level));
        PlayerPrefs.Save();
    }

    public static void SetLastScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            PlayerPrefs.SetString(LastSceneKey, sceneName);
            PlayerPrefs.Save();
        }
    }

    public static string GetLastScene(string fallbackScene)
    {
        string scene = PlayerPrefs.GetString(LastSceneKey, fallbackScene);
        return string.IsNullOrEmpty(scene) ? fallbackScene : scene;
    }

    public static void SaveProgress(string sceneName, int maxUnlockedLevel)
    {
        SetHasSave(true);
        SetLastScene(sceneName);
        SetMaxUnlockedLevel(maxUnlockedLevel);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(HasSaveKey);
        PlayerPrefs.DeleteKey(MaxUnlockedLevelKey);
        PlayerPrefs.DeleteKey(LastSceneKey);
        PlayerPrefs.Save();
    }
}
