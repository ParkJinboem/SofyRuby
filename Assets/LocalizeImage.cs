using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LocalizeImage : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public SystemLanguage systemLanguage;
        public Sprite sp;
    }

    [SerializeField] List<Data> datas;

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void Start()
    {
        Data data = datas.Find(x => x.systemLanguage == Application.systemLanguage);        
        if (data != null)
        {
            img.sprite = data.sp;
        }
        else
        {
            img.sprite = datas[0].sp;
        }
    }
}
