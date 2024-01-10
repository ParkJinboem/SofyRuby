using UnityEngine;
using TMPro;

public class LocalizeText : MonoBehaviour
{
    public string localizeKey;

    public TextMeshProUGUI Text { get; set; }

    public string Localize
    {
        get { return localizeKey; }
        set { localizeKey = value; Localization(); }
    }

    private void Awake()
    {
        Text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void Start()
    {
        if (!string.IsNullOrEmpty(localizeKey))
        {
            Localization();
        }
        // 현재 미사용, 씬 재로드
        //LocalizeManager.LocalizeChanged += Localization;
    }

    public void OnDestroy()
    {
        // 현재 미사용, 씬 재로드
        //LocalizeManager.LocalizeChanged -= Localization;
    }

    private void Localization()
    {
        if (Text != null &&
            !string.IsNullOrEmpty(localizeKey))
        {
            Text.text = LocalizeManager.Instance.Localization(Localize);
        }
    }
}
