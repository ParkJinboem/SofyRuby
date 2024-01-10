using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothCanvas : MonoBehaviour
{
    [SerializeField] GameObject[] noCaptureObjs;

    [Header("Ads")]
    [SerializeField] private RectTransform bannerCanvasRoot;
    [SerializeField] private RectTransform[] bannerChangeTransforms; // 배너 광고로 위치 변경할 오브젝트 리스트

    private void Awake()
    {
        AdsManager.OnLoadedBanner += OnLoadedBanner;
        ClothEvent.OnSaveLoad += HandlerSaveLoad;
    }

    private void Start()
    {
        MakingStairBtn m_stair = FindObjectOfType<MakingStairBtn>();
        m_stair._myBtn[1].gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        AdsManager.OnLoadedBanner -= OnLoadedBanner;
        ClothEvent.OnSaveLoad -= HandlerSaveLoad;
    }

    public void OnClickCompleteBtn()
    {
        FindObjectOfType<IntroCanvas>().OnClickSaveBtn();
    }

    #region Actions-Events
    public void OnLoadedBanner(bool isShow)
    {
        for (int i = 0; i < bannerChangeTransforms.Length; i++)
        {
            Vector2 position = bannerChangeTransforms[i].offsetMax;
            position.y = isShow ? -AdsManager.Instance.GetBannerHeight(bannerCanvasRoot.rect) : 0f;
            bannerChangeTransforms[i].offsetMax = position;
        }
    }

    private void HandlerSaveLoad(ClothEvent.SaveLoadState saveLoadState)
    {
        switch (saveLoadState)
        {
            case ClothEvent.SaveLoadState.Open:
                for (int i = 0; i < noCaptureObjs.Length; i++)
                {
                    noCaptureObjs[i].SetActive(false);
                }
                break;
            case ClothEvent.SaveLoadState.Close:
                for (int i = 0; i < noCaptureObjs.Length; i++)
                {
                    noCaptureObjs[i].SetActive(true);
                }
                break;
        }
    }
    #endregion
}
