using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Pimples : MonoBehaviour
{
    public RectTransform[] _pimples;

    void Start()
    {
        _pimples = GetComponentsInChildren<RectTransform>();        
    }

    public RectTransform[] GetPimples()
    {
        return _pimples;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        for (int i = 0; i < _pimples.Length; i++)
        {
            _pimples[i].gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }
}
