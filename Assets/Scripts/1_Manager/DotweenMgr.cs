using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class DotweenMgr : MonoBehaviour
{
    public static bool isTweening;

    public static void DoPopupOpen(float startVal, float endVal, float time, Transform targetObj)
    {
        isTweening = true;
        Vector3 startScale = new Vector3(startVal, startVal, 1);
        targetObj.localScale = startScale;
        targetObj.DOScale(endVal, time).SetEase(Ease.OutBack).OnComplete(() =>
        {
            isTweening = false;
        });
    }

    public static IEnumerator DoFade(float endVal, float time, GameObject targetObj)
    {
        targetObj.SetActive(true);
        isTweening = true;
        Image popupImage = targetObj.GetComponent<Image>();
        popupImage.color = new Color(.3f, .3f, .3f, 0);
        popupImage.DOFade(endVal, time);
        yield return new WaitForSeconds((time) + 1f);
        popupImage.DOFade(0f, time).OnComplete(() =>
        {
            targetObj.SetActive(false);
            isTweening = false;
        });
    }

    public static void DoFadeImage(float startAlpha, float endAlpha, float time, GameObject targetObj)
    {
        targetObj.SetActive(true);
        isTweening = true;
        Image popupImage = targetObj.GetComponent<Image>();
        popupImage.color = new Color(popupImage.color.r, popupImage.color.g, popupImage.color.b, startAlpha);
        popupImage.DOFade(endAlpha, time).OnComplete(() =>
        {
            isTweening = false;
        });
    }
    public static void DoFadeImage(float startAlpha, float endAlpha, float time, RawImage targetObj)
    {
        targetObj.gameObject.SetActive(true);
        isTweening = true;
        RawImage popupImage = targetObj.GetComponent<RawImage>();
        popupImage.color = new Color(popupImage.color.r, popupImage.color.g, popupImage.color.b, startAlpha);
        popupImage.DOFade(endAlpha, time).OnComplete(() =>
        {
            isTweening = false;
        });
    }
   

    public static void DoSizeImage(float startScale, float endScale, float time, GameObject targetObj)
    {
        targetObj.SetActive(true);
        isTweening = true;
        targetObj.transform.localScale = new Vector3(startScale, startScale, 1);
        targetObj.transform.DOScale(endScale, time).OnComplete(() =>
        {
            isTweening = false;
        });
    }

    public static IEnumerator DoTextFade(float endVal, float time, GameObject targetObj)
    {
        targetObj.SetActive(true);
        isTweening = true;
        Text popupImage = targetObj.GetComponent<Text>();
        popupImage.DOFade(endVal, time);
        yield return new WaitForSeconds((time) + .8f);
        popupImage.DOFade(0f, time).OnComplete(() =>
        {
            targetObj.SetActive(false);
            isTweening = false;
        });
    }

    public static void DoLocalMoveX(float x, float time, Transform moveObj)
    {
        isTweening = true;

        moveObj.DOLocalMoveX(x, time).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            isTweening = false;
        });
    }
    public static void DoLocalMoveX(float x, float time, RectTransform moveObj)
    {
        isTweening = true;

        moveObj.DOAnchorPosX(x, time).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            isTweening = false;
        });
    }

    public static void DoLocalMoveY(float y, float time, Transform moveObj)
    {
        isTweening = true;

        moveObj.DOLocalMoveY(y, time).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            isTweening = false;
        });
    }
}
