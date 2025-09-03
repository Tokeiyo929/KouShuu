using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using TMPro;
using UnityEngine;

public class LocalizationText : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string chineseText;
    [TextArea(3, 10)]
    [SerializeField] private string englishText;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Global.CurrentLanguage.RegisterWithInitValue(OnLanguageChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnLanguageChanged(GlobalEnums.Language language)
    {
        if (language == GlobalEnums.Language.Chinese)
        {
            text.text = chineseText;
        }
        else
        {
            text.text = englishText;
        }
    }

}
