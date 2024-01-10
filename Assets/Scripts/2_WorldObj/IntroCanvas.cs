using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

/// <summary>
/// 캔버스 전역 사용
/// </summary>
public class IntroCanvas : MonoBehaviour
{
    public GameObject InAppPop; // ㅈ쪼그만 캐릭터 아이콘 누름 팝업
    public GameObject InAppContent;
    public GameObject m_SettingPannel; // 상단 세팅관련 버튼 모음
    public GameObject m_SettingBtn; // 세팅 버튼
    public GameObject m_SettingPop; // 세팅 온오프
    public GameObject m_SmallSettingPop; // 인트로씬, 메인씬에서 팝업창 열때

    public GameObject m_LoadingPannel; // 로딩패널
    public GameObject m_IntroPannel; // 인트로씬 등장 패널
    public GameObject m_MakingStairPannel; // 다음/이전 버튼 (메이킹씬에서 화장 단계등)
    public GameObject m_SkinSelectPannel;

    public GameObject m_SaveLoadPannel;

    public GameObject m_ExitPop_BG;
    public GameObject m_ExitPop_Pannel;
    public LocalizeText m_ExitPop_LocalizeText;

    public GameObject m_AppInfo_BG;
    public GameObject m_AppInfo_Pannel;
    public LocalizeText m_AppInfo_LocalizeText;

    public GameObject m_ADVPannel;
    public GameObject m_ADVPop;

    public GameObject appReviewAlertView;
    public LocalizeText appReviewLocalizeText;

    public AlertView alertView;
    public MessageAlertView messageAlertView;

    public ParticleSystem[] skinParticles;

    [SerializeField]
    private bool _checkMakingScene = false;

    bool ExitCheck = false;

    [Header("Ads")]
    [SerializeField] private RectTransform bannerCanvasRoot;
    [SerializeField] private RectTransform[] bannerChangeTransforms; // 배너 광고로 위치 변경할 오브젝트 리스트

    public void Awake()
    {
        DontDestroyOnLoad(this);

        AdsManager.OnLoadedBanner += OnLoadedBanner;
    }

    private void OnDestroy()
    {
        AdsManager.OnLoadedBanner -= OnLoadedBanner;
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
    #endregion

    private void OnLevelWasLoaded(int level)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

    }


    public void OnInAppSelect(bool _OnOff) // 인트로씬에서 쪼끄만 캐릭터 아이콘 온오프
    {
        SoundMgr.GetInst().OnClickSoundBtn();

        OnClickSetting(false);
        InAppPop.SetActive(_OnOff);
        if (_OnOff) DotweenMgr.DoPopupOpen(.0f, 1f, .4f, InAppContent.transform);
    }

    public void OnClickOpenSetting()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        OnClickSetting(true);
    }

    public void OnClickCloseSetting()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        OnClickSetting(false);
    }

    public void OnClickSetting(bool _OnOff) // 설정 눌렀을때 온오프
    {
        if (SceneManager.GetActiveScene().name != GameManager.ClothSceneName)
        {
            m_SmallSettingPop.SetActive(_OnOff);
            m_SettingBtn.SetActive(!_OnOff);
            //if (_OnOff) DotweenMgr.DoPopupOpen(.9f, 1f, .3f, m_SmallSettingPop.transform);
        }
        else
        {
            m_SettingPop.SetActive(_OnOff);
            m_SettingBtn.SetActive(!_OnOff);
            //if (_OnOff) DotweenMgr.DoPopupOpen(.9f, 1f, .3f, m_SettingPop.transform);
        }
    }

    public void OnClickStart()
    {
        SoundMgr.GetInst().OnClickSoundBtn();

        OnClickSetting(false);
        m_LoadingPannel.SetActive(true);
        m_SettingPannel.SetActive(false);

        GameManager.Instance.InitAds(() =>
        {
            SceneManager.LoadScene(GameManager.MainSceneName);
        });
    }

    int tempSkinNumber = 0;
    public void OnClickSkinNumber(int _idx)
    {
        SoundMgr.GetInst().OnPlayOneShot("se_01");
        tempSkinNumber = _idx;
    }
    public void OnClickSkinEff(GameObject _obj)
    {
        foreach(ParticleSystem particle in skinParticles)
        {
            particle.Stop();
        }
        _obj.GetComponent<ParticleSystem>().Play();

        WorldMgr.GetInst().OnOffLoadPannel(true);
        Invoke("OnClickSkinSelect", .5f);
    }

    public void OnClickSkinSelect()
    {
        try
        {
            OnClickSetting(false);
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

            WorldMgr.GetInst().SetJujuSkin(tempSkinNumber);
            m_SkinSelectPannel.SetActive(false);
            MainSceneMgr.GetInst().OnWorldCanvasCalled();
            _checkMakingScene = true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    public void OnOffLoadingPannel()
    {
        m_LoadingPannel.SetActive(!m_LoadingPannel.activeSelf);
    }

    public void OnHomeBtn()
    {
        ExitCheck = false;
        SoundMgr.GetInst().OnClickSoundBtn();
        m_ExitPop_LocalizeText.Localize = "MoveMainPop";
        m_ExitPop_BG.SetActive(true);
        DotweenMgr.DoPopupOpen(.0f, 1f, .4f, m_ExitPop_Pannel.transform);
    }

    public void OnExitBtn()
    {
        ExitCheck = true;
        SoundMgr.GetInst().OnClickSoundBtn();
        m_ExitPop_LocalizeText.Localize = "ExitPop";
        m_ExitPop_BG.SetActive(true);
        DotweenMgr.DoPopupOpen(.0f, 1f, .4f, m_ExitPop_Pannel.transform);
    }

    public void OnClickExitBtn()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        if (!ExitCheck)
        {
            ResetIntro();
            _checkMakingScene = false;
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    public void OnClickExitBtn_Non()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        ExitCheck = false;
        m_ExitPop_BG.SetActive(false);
    }
    public void OnClickSaveBtn()
    {
        SoundMgr.GetInst().OnClickSoundBtn();

        m_SettingBtn.SetActive(false);
        m_SettingPop.SetActive(false);
        m_SaveLoadPannel.SetActive(true);
        DotweenMgr.DoPopupOpen(.6f, 1f, .3f, m_SaveLoadPannel.transform);

        SaveLoadMgr.GetInst().OnClickSLPannel();

        ClothEvent.SaveLoad(ClothEvent.SaveLoadState.Open);
    }
    [SerializeField]
    Image[] _MakingStairImgs;

    OHBtnChecker _ohbtn;
    public void SetOhBtnObj(OHBtnChecker _obj) => _ohbtn = _obj;
    bool _checker = false;
    public void SetSLChecker(bool _isLoaded) => _checker = _isLoaded; // SL패널 영향으로 사이드바에 영향을 줬는지 체크
    bool _checker2 = false;
    public void SetChecker2(bool _check) => _checker2 = _check; // SL패널에서 사이드바 닫았는지 체크


    public void OnClickExitBtn_SL()
    {
        WorldMgr.GetInst().OnOffOptionPannel(false);
        //SaveLoadMgr.GetInst().SettingInit();
        switch (SceneManager.GetActiveScene().name)
        {
            case GameManager.IntroSceneName:
                break;
            case GameManager.MainSceneName:
                if (!(_MakingStairImgs.Length > 0))
                {
                    MakingStairBtn _tempObj = FindObjectOfType<MakingStairBtn>();
                    _MakingStairImgs = _tempObj.GetComponentsInChildren<Image>();
                }
                foreach (Image _img in _MakingStairImgs) _img.enabled = true;
               
                if (_ohbtn != null)
                {
                    MakingCanvas _make = FindObjectOfType<MakingCanvas>();
                    _make.OnOffBtn();
                    _ohbtn.gameObject.SetActive(true);
                }
                break;
            case GameManager.ClothSceneName:
                if (!(_MakingStairImgs.Length > 0))
                {
                    MakingStairBtn _tempObj = FindObjectOfType<MakingStairBtn>();
                    _MakingStairImgs = _tempObj.GetComponentsInChildren<Image>();
                }
                foreach (Image _img in _MakingStairImgs) _img.enabled = true;
                break;
        }
     

        SoundMgr.GetInst().OnClickSoundBtn();
        m_SettingBtn.SetActive(true);
        //m_SettingPop.SetActive(true);
        m_SaveLoadPannel.SetActive(false);

        ClothEvent.SaveLoad(ClothEvent.SaveLoadState.Close);
    }
    private void ResetIntro()
    {
        AdsManager.Instance.ActiveBanner(false);

        Destroy(this.gameObject);
        Destroy(WorldMgr.GetInst().gameObject);
        Destroy(SaveLoadMgr.GetInst().gameObject);
        Destroy(SoundMgr.GetInst()._BgmSource);
        Destroy(SoundMgr.GetInst().gameObject);

        SceneManager.LoadScene(GameManager.IntroSceneName);
    }
    public void OnClickAppInfo()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        m_AppInfo_BG.SetActive(true);
        m_AppInfo_LocalizeText.Text.text = string.Format(LocalizeManager.Instance.Localization("AppInfo"), Application.version);
        DotweenMgr.DoPopupOpen(0f, 1f, .4f, m_AppInfo_Pannel.transform);
    }
    public void OnClickAppInfo_Exit()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        m_AppInfo_BG.SetActive(false);
    }

    public bool GetCheckMakingScene() { return _checkMakingScene; }

    public void OnClickADVPannel(bool _isActive)
    {
        if (_isActive)
        {
            DotweenMgr.DoPopupOpen(0f, 1f, .4f, m_ADVPop.transform);
        }
        m_ADVPannel.SetActive(_isActive);
    }

    public void ShowMessageAlert(string message)
    {
        messageAlertView.Show(message);
    }

    public void ShowAlert(AlertViewOptions avo)
    {
        alertView.Show(avo);
    }

    public void ShowMakingStairPannel()
    {
        m_MakingStairPannel.SetActive(true);
    }

    public void ShowAppReviewAlertView()
    {
        appReviewAlertView.SetActive(true);
        string message;
        #if PLATFORM_IOS
            message = "RequestReviewAppStore";
        #else
            message = "RequestReviewGooglePlay";
        #endif
        appReviewLocalizeText.Localize = message;
    }

    public void ShowAppReview()
    {
        AppReviewManager.Instance.ShowAppReview();
    }
}
