using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SLPannel : MonoBehaviour
{
    [SerializeField]
    private Button[] _slots;

    private void OnEnable()
    {
        _slots = GetComponentsInChildren<Button>();
        foreach (Button _btn in _slots)
        {
            _btn.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture2D>("SaveSlotIdx" + _btn.GetComponentInChildren<Text>().text);
        }
    }
}
