using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public enum Language
    {
        Spanish = 0,
        English = 1,
        Japanese = 2
    }

    private const string LanguageKey = "Language";

    public static LanguageManager Instance { get; private set; }

    public static Language CurrentLanguage
    {
        get
        {
            int value = PlayerPrefs.GetInt(LanguageKey, (int)Language.Spanish);
            return (Language)Mathf.Clamp(value, 0, 2);
        }
    }

    public event Action<Language> LanguageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLanguage(Language language)
    {
        PlayerPrefs.SetInt(LanguageKey, (int)language);
        PlayerPrefs.Save();
        LanguageChanged?.Invoke(language);
    }

    public void SetSpanish()
    {
        SetLanguage(Language.Spanish);
    }

    public void SetEnglish()
    {
        SetLanguage(Language.English);
    }

    public void SetJapanese()
    {
        SetLanguage(Language.Japanese);
    }
}
