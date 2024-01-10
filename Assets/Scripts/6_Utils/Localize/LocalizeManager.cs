using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

[Serializable]
public class LocalizeLanguage
{
    public SystemLanguage systemLanguage;
    public string key;
}

public class LocalizeManager : MonoBehaviour
{
    public static LocalizeManager Instance = null;

    public static event Action LocalizeChanged = () => { };

    [SerializeField] private LocalizeLanguage gameLanguage;
    public TextAsset textAsset;
    public List<LocalizeLanguage> localizeLanguages = new List<LocalizeLanguage>();
    private Dictionary<string, Dictionary<string, string>> offlineTexts = new Dictionary<string, Dictionary<string, string>>(); // key : language, inner key : title
    private Dictionary<string, Dictionary<string, string>> texts = new Dictionary<string, Dictionary<string, string>>(); // key : language, inner key : title

    public LocalizeLanguage GameLanguage
    {
        get { return gameLanguage; }
        set
        {
            gameLanguage = value;
            LocalizeChanged();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        Init();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public void Init()
    {
        gameLanguage = localizeLanguages[0];
        for (int i = 0; i < localizeLanguages.Count; i++)
        {
            if (localizeLanguages[i].systemLanguage == Application.systemLanguage)
            {
                gameLanguage = localizeLanguages[i];
                break;
            }
        }
        GameLanguage = gameLanguage;

        Read();
    }

    /// <summary>
    /// 오프라인 csv 읽어오기
    /// </summary>
    private void Read()
    {
        if (offlineTexts.Count > 0)
        {
            return;
        }

        var text = textAsset.text.Replace("[Newline]", "\n");
        var matches = Regex.Matches(text, "\"[\\s\\S]+?\"");
        foreach (Match match in matches)
        {
            text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[comma]").Replace("\n", "[newline]"));
        }

        int startKeyIndex = 0;
        int startLanguageIndex = 1;
        var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();
        for (var i = startLanguageIndex; i < languages.Count; i++)
        {
            if (!offlineTexts.ContainsKey(languages[i]))
            {
                offlineTexts.Add(languages[i], new Dictionary<string, string>());
            }
        }
        for (var i = 1; i < lines.Length; i++)
        {
            var columns = lines[i].Split(',').Select(j => j.Trim()).Select(j => j.Replace("[comma]", ",").Replace("[newline]", "\n")).ToList();
            var key = columns[startKeyIndex];
            for (var j = startLanguageIndex; j < languages.Count; j++)
            {
                offlineTexts[languages[j]].Add(key, columns[j]);
            }
        }
    }

    /// <summary>
    /// 온라인 데이터 저장
    /// </summary>
    /// <param name="texts"></param>
    public void SetTexts(Dictionary<string, Dictionary<string, string>> texts)
    {
        this.texts = texts;
    }

    /// <summary>
    /// 로컬라이즈
    /// </summary>
    /// <param name="localizeKey"></param>
    /// <returns></returns>
    public string Localization(string localizeKey)
    {
        return Localization(GameLanguage, localizeKey);
    }

    /// <summary>
    /// 로컬라이즈
    /// </summary>
    /// <param name="localizeKey"></param>
    /// <returns></returns>
    private string Localization(LocalizeLanguage localizeLanguage, string localizeKey)
    {
        if (texts.ContainsKey(localizeLanguage.key))
        {
            if (texts[localizeLanguage.key].ContainsKey(localizeKey))
            {
                return texts[localizeLanguage.key][localizeKey];
            }
        }
        if (offlineTexts.ContainsKey(localizeLanguage.key))
        {
            if (offlineTexts[localizeLanguage.key].ContainsKey(localizeKey))
            {
                return offlineTexts[localizeLanguage.key][localizeKey];
            }
        }
        return null;
    }
}
