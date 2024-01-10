using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SlotController : MonoBehaviour
{
    public Image imgSelectBg;
    public Image imgStar;
    public Sprite spStarN, spStarP;
    public Image imgWhiteFrame;
    public Color colorWhiteFrameN, colorWhiteFrameP;

    public void SelectSlot(bool isSelected)
    {
        imgStar.sprite = isSelected ? spStarP : spStarN;
        //imgWhiteFrame.color = isSelected ? colorWhiteFrameP : colorWhiteFrameN;
        imgSelectBg.gameObject.SetActive(isSelected);
    }
}
