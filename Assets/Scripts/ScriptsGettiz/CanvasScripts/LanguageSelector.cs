using System;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public enum Languages
    {
        English,
        Spanish,
        Japanese
    }

    public static Languages CurrentLanguage = Languages.Spanish;

    public static event Action OnLanguageChanged;

    public void SetEnglish() => ChangeLanguage(Languages.English);
    public void SetSpanish() => ChangeLanguage(Languages.Spanish);
    public void SetJapanese() => ChangeLanguage(Languages.Japanese);

    private void ChangeLanguage(Languages target)
    {
        if (CurrentLanguage == target) return;

        CurrentLanguage = target;
        
        OnLanguageChanged?.Invoke();

        Debug.Log("language changed to" + target);
    }
}

[Serializable]
public struct TranslationLabel
{
    public LanguageSelector.Languages language;

    [TextArea(3, 10)]
    public string translatedLabelText;
}