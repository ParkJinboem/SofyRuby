using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainSceneMgr : MonoBehaviour
{
    #region SingleTon
    private static MainSceneMgr _inst;
    private void Awake()
    {
        if (_inst == null) _inst = this;
    }
    public static MainSceneMgr GetInst()
    {
        if (_inst == null)
        {
            Debug.LogError("Main Scene Mgr Instance is NULL"); return null;
        }
        else return _inst;
    }
    #endregion



    [SerializeField]
    private IntroCanvas _WorldCanvas;

    [SerializeField]
    private MakingCanvas _MakingCanvas;

    public void Start()
    {
        Resources.UnloadUnusedAssets();
        Invoke("SetOffLoadingPannel", 1f);
    }
    private void SetOffLoadingPannel()
    {
        if(_WorldCanvas == null)
        _WorldCanvas = FindObjectOfType<IntroCanvas>();
        _WorldCanvas.m_LoadingPannel.SetActive(false);
        _WorldCanvas.m_IntroPannel.SetActive(false);
        _WorldCanvas.m_SettingPannel.SetActive(true);

        if (!SaveLoadMgr.GetInst().CheckSceneLoad() &&
            !WorldMgr.GetInst().GetPrevSceneChecker())
        {
            _WorldCanvas.m_SkinSelectPannel.SetActive(true);
            CheckReview();
        }

        bool _testCheck = WorldMgr.GetInst().GetPrevSceneChecker();

        if (WorldMgr.GetInst().GetPrevSceneChecker())
        {
            for(int i = 0;i < WorldMgr.GetInst().GetMaxToolIdx(); i++)
            {
                _WorldCanvas.m_MakingStairPannel.GetComponent<MakingStairBtn>().Next_Btn();
            }
            WorldMgr.GetInst().SetPrevSceneChecker(false);
            WorldMgr.GetInst().SetClothSceneJuju();
        }

        if (WorldMgr.GetInst().GetCheckLoadmainScene())
        {

            if (!_testCheck)
            {

                for (int i = 0; i < WorldMgr.GetInst().GetmainSceneIdx(); i++)
                {
                    _WorldCanvas.m_MakingStairPannel.GetComponent<MakingStairBtn>().Next_Btn();
                }
            }


            if (WorldMgr.GetInst().GetmainSceneIdx() < 3)
            {
                FaceMaskChecker[] _Faces = FindObjectsOfType<FaceMaskChecker>();

                foreach (FaceMaskChecker _face in _Faces)
                {
                    switch (_face.GetType().Name)
                    {
                        case "FaceMaskChecker_Cheek":
                            AdvancedMobilePaint.AdvancedMobilePaint _paint_dirty_Cheek = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
                            _paint_dirty_Cheek.gameObject.GetComponent<RawImage>().enabled = false;
                            _paint_dirty_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                            break;
                        case "FaceMaskChecker_Eyebrow":
                            AdvancedMobilePaint.AdvancedMobilePaint _paint_dirty_Eyebrow = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
                            _paint_dirty_Eyebrow.gameObject.GetComponent<RawImage>().enabled = false;
                            _paint_dirty_Eyebrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                            break;
                        case "FaceMaskChecker_LipStick":
                            AdvancedMobilePaint.AdvancedMobilePaint _paint_dirty_Lipstick = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
                            _paint_dirty_Lipstick.gameObject.GetComponent<RawImage>().enabled = false;
                            _paint_dirty_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;
                            break;
                        case "FaceMaskChecker_Shadow":
                            AdvancedMobilePaint.AdvancedMobilePaint _paint_dirty_Shadow = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
                            _paint_dirty_Shadow.gameObject.GetComponent<RawImage>().enabled = false;
                            _paint_dirty_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                            break;
                        default:
                            AdvancedMobilePaint.AdvancedMobilePaint _paint_dirty = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
                            break;

                    }
                }
            }
        }

        // SL에서 인덱스 넘겨준거 받아서 로딩화면 종료될때 스왑보드 인덱스 조정하기 

        if (SaveLoadMgr.GetInst().CheckSceneLoad())
        {
            SaveLoadMgr.GetInst().SetCheckSceneLoad(false);
            SaveLoadMgr.GetInst().LoadData();
            SaveLoadMgr.GetInst().Set_isEqual(false);
        }

        // 로딩 완료 후 배너 광고
        AdsManager.Instance.RequestBanner();
        AdsManager.Instance.WatchAdWithFuncUse("AdsPassSelectSkin");
    }

    public void OnWorldCanvasCalled() => Invoke("LoadGirlHolder", .5f);

    public void LoadGirlHolder()
    {
        if (_MakingCanvas == null) _MakingCanvas = FindObjectOfType<MakingCanvas>();
        int skinidx = WorldMgr.GetInst().GetJujuSkin();
        Sprite[] texs = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.SKIN);
        Image[] _obj = _MakingCanvas.GetComponentInChildren<GirlHolder>().GetComponentsInChildren<Image>();
        Image _skin = null;
        foreach(Image _img in _obj)
        {
            if (_img.name == "Skin") _skin = _img;
        }
        if (_skin == null)
        {
            Debug.LogError("Can not found Girl Skin");
            return;
        }
        _skin.sprite = texs[skinidx];
        SaveLoadMgr.GetInst().SettingInfo_Skin(skinidx);
       
        _WorldCanvas.OnOffLoadingPannel();

        // 로딩 완료 후 배너 광고
        AdsManager.Instance.RequestBanner();
        AdsManager.Instance.WatchAdWithFuncUse("AdsPassMakeup");

        _WorldCanvas.ShowMakingStairPannel();
    }

    public void LoadGirlHolderOnLoaded()
    {
        if (_MakingCanvas == null) _MakingCanvas = FindObjectOfType<MakingCanvas>();
        int skinidx = WorldMgr.GetInst().GetJujuSkin();
        Sprite[] texs = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.SKIN);
        Image[] _obj = _MakingCanvas.GetComponentInChildren<GirlHolder>().GetComponentsInChildren<Image>();
        Image _skin = null;
        foreach (Image _img in _obj)
        {
            if (_img.name == "Skin") _skin = _img;
        }
        if (_skin == null)
        {
            Debug.LogError("Can not found Girl Skin");
            return;
        }
        _skin.sprite = texs[skinidx];
        SaveLoadMgr.GetInst().SettingInfo_Skin(skinidx);
    }

    /// <summary>
    /// 평점 요청 확인
    /// </summary>
    private void CheckReview()
    {
        if (AppReviewManager.Instance.CheckAppReview())
        {
            _WorldCanvas.ShowAppReviewAlertView();
        }
    }
}
