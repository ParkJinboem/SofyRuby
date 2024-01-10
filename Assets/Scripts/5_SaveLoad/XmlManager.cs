using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmlManager : MonoBehaviour
{
    #region Singlton
    private static XmlManager _inst;
    public static XmlManager GetInst()
    {
        if (_inst != null) return _inst;
        else
        {
            Debug.LogError("XML Maanger is NULL"); return null;
        }
    }

    private void Awake()
    {
        if (_inst == null) _inst = this;
        else Debug.LogError("XML Manager Already Setted");
        DontDestroyOnLoad(this);
    }
    #endregion


    private int _eyeIdx;
    private int _hairIdx;
    
    
    void FirstSettings()
    {

    }
    void OnSave()
    {

    }
    void OnLoad()
    {

    }

    void SettingInformation_Eye(int _idx) { _eyeIdx = _idx; }
    void SettingInformation_Hair(int _idx) { _hairIdx = _idx; }
}
