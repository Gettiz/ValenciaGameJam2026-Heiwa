using UnityEngine;
using UnityEngine.UI;

#if TMP_PRESENT
using TMPro;
#endif

public class LocalizedText : MonoBehaviour
{
    [Header("Text")]
    [TextArea] public string spanish;
    [TextArea] public string english;
    [TextArea] public string japanese;

    [Header("References")]
    [SerializeField] private Text uiText;

#if TMP_PRESENT
    [SerializeField] private TMP_Text tmpText;
#endif

    private void Awake()
    {
        if (uiText == null)
        {
            uiText = GetComponent<Text>();
        }

#if TMP_PRESENT
        if (tmpText == null)
        {
            tmpText = GetComponent<TMP_Text>();
        }
#endif
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.LanguageChanged += OnLanguageChanged;
        }

        Apply(LanguageManager.CurrentLanguage);
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.LanguageChanged -= OnLanguageChanged;
        }
    }

    private void OnLanguageChanged(LanguageManager.Language language)
    {
        Apply(language);
    }

    private void Apply(LanguageManager.Language language)
    {
        string value = GetValue(language);

        if (uiText != null)
        {
            uiText.text = value;
        }

#if TMP_PRESENT
        if (tmpText != null)
        {
            tmpText.text = value;
        }
#endif
    }

    private string GetValue(LanguageManager.Language language)
    {
        switch (language)
        {
            case LanguageManager.Language.English:
                return string.IsNullOrEmpty(english) ? spanish : english;
            case LanguageManager.Language.Japanese:
                return string.IsNullOrEmpty(japanese) ? spanish : japanese;
            default:
                return spanish;
        }
    }
}
