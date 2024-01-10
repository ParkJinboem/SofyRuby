using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HolderIdx : MonoBehaviour
{
    [SerializeField]
    private Image[] _floweridx;

    public Sprite _WhiteF;
    public Sprite _PupleF;

    private void Start()
    {
       _floweridx= GetComponentsInChildren<Image>();
    }

    public void SetFlowerIdx(int _idx)
    {
        for(int i = 0; i< _floweridx.Length; i++)
        {
            if (i == _idx)
            {
                _floweridx[i].sprite = _WhiteF;
                DotweenMgr.DoSizeImage(.3f, 1f, .4f, _floweridx[i].gameObject);
            }
            else
            {
                _floweridx[i].sprite = _PupleF;
                _floweridx[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
