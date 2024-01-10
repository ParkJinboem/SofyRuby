using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DataSetup : MonoBehaviour
{
    public static DataSetup Instance = null;

    [Serializable]
    public class LockData
    {
        public MenuSelector ms;
        public int idx;
    }

    [SerializeField] TextAsset ta;
    [SerializeField] List<LockData> clothLockDatas;

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
    }

    public bool IsLock(MenuSelector ms, int idx)
    {
        return clothLockDatas.Find(x => x.ms == ms && x.idx == idx) != null;
    }

    #if UNITY_EDITOR
    public void Setup()
    {
        CreateData(ta);
        EditorUtility.SetDirty(this);
    }

    private void CreateData(TextAsset ta)
    {
        clothLockDatas = new List<LockData>();
        List<Dictionary<string, string>> results = Parse(ta);
        for (int i = 0; i < results.Count; i++)
        {
            clothLockDatas.Add(new LockData()
            {
                ms = (MenuSelector)int.Parse(results[i]["ms"]),
                idx = int.Parse(results[i]["idx"])
            });
        }
    }

    private List<Dictionary<string, string>> Parse(TextAsset ta)
    {
        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();

        var text = ta.text.Replace("[Newline]", "\n");
        var matches = Regex.Matches(text, "\"[\\s\\S]+?\"");
        foreach (Match match in matches)
        {
            text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[comma]").Replace("\n", "[newline]"));
        }

        var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        var keys = lines[0].Split(',').Select(i => i.Trim()).ToList();
        for (var i = 1; i < lines.Length; i++)
        {
            var columns = lines[i].Split(',').Select(j => j.Trim()).Select(j => j.Replace("[comma]", ",").Replace("[newline]", "\n")).ToList();
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int j = 0; j < columns.Count; j++)
            {
                result.Add(keys[j], columns[j]);
            }
            results.Add(result);
        }

        return results;
    }
    #endif

    private void GetFiles()
    {
        //folderPaths = Directory.GetDirectories(AssetDatabase.GetAssetPath(clothRoot.GetInstanceID()));

        //results = new List<Result>();
        //for (int i = 0; i < folderPaths.Length; i++)
        //{
        //    string folderName = Path.GetFileName(folderPaths[i]);
        //    TextAsset ta = tas.Find(x => x.name == folderName);
        //    if (ta != null)
        //    {
        //        CreateResult(ta);
        //    }
        //}

        //string[] dataPaths = Directory.GetFiles(AssetDatabase.GetAssetPath(dataRoot.GetInstanceID()));
        //for (int i = 0; i < dataPaths.Length; i++)
        //{
        //    TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(dataPaths[i]);
        //    if (ta != null)
        //    {

        //    }
        //}
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DataSetup))]
public class DataSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DataSetup generator = (DataSetup)target;
        if (GUILayout.Button("Setup"))
        {
            generator.Setup();
        }
    }
}
#endif