using TMPro;
using UnityEngine;

public class TextMeshProLanguages : MonoBehaviour
{
    [Header("Translations")]
    [SerializeField] private TranslationLabel[] translationsLabel;

    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        LanguageSelector.OnLanguageChanged += UpdateTranslationLabel;
        UpdateTranslationLabel();
    }

    private void OnDisable()
    {
        LanguageSelector.OnLanguageChanged -= UpdateTranslationLabel;
    }

    public void UpdateTranslationLabel()
    {
        if (textComponent == null) return;
        
        textComponent.text = GetTranslatedText(LanguageSelector.CurrentLanguage, translationsLabel);
    }
    
    private string GetTranslatedText(LanguageSelector.Languages targetLanguage, TranslationLabel[] labels)
    {
        if (labels == null || labels.Length == 0)
        {
            return "no text found";
        }

        foreach (TranslationLabel item in labels)
        {
            if (item.language == targetLanguage)
            {
                return item.translatedLabelText;
            }
        }

        return "translation not found";
    }
}