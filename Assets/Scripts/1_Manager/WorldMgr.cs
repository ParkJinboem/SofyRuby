using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// 전역적으로 사용하는 매니저 : 앱 세팅, 잠금해제 정보, 씬 전환시 필요 정보 관
/// </summary>
public class WorldMgr : MonoBehaviour
{
    #region SingleTon
    private static WorldMgr _inst;
    public static WorldMgr GetInst()
    {
        if (_inst == null)
        {
            Debug.LogError("WorldMgr Instatnce is NULL"); return null;
        }
        else return _inst;
    }
    #endregion

    /// <summary>
    /// 스킨 정보 인덱스 (노말, 라이트베이지, 다크 베이지)
    /// </summary>
    private int SelectedJujuSkin;
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        if (_inst == null)
        {
            Application.targetFrameRate = 60;
            _inst = this;
        }
        else Debug.LogError("Already instatnce Not NULL");
        DontDestroyOnLoad(this);
    }

    //Cloth _clothData; // 옷 해금 정보 저장용
    //string _clothDataStr = "Cloth";
    //public string GetClothStr() { return _clothDataStr; }

    private bool _checkPrevScene = false;

    private int _maxToolIdx;

    public void SetPrevSceneChecker(bool _set) => _checkPrevScene = _set;
    public bool GetPrevSceneChecker() { return _checkPrevScene; }


    private bool _checkLoadMainScene = false;
    public void SetCheckLoadMainScene(bool _set) => _checkLoadMainScene = _set;
    public bool GetCheckLoadmainScene() { return _checkLoadMainScene; }

    /// <summary>
    /// 기존에는 화장 씬에서 저장하는 것도 존재, 저장 로드시 어디서 저장했는지 인덱스로 저장
    /// </summary>
    [SerializeField]
    private int _MainSceneIdx;
    public void SetMainSceneIdx(int _idx) => _MainSceneIdx = _idx;
    public int GetmainSceneIdx() { return _MainSceneIdx; }

    public int GetJujuSkin() { return SelectedJujuSkin; }
    public void SetJujuSkin(int _idx) => SelectedJujuSkin = _idx;

    IntroCanvas _introCv;
    private void Start()
    {
        _introCv = FindObjectOfType<IntroCanvas>();
    }

    public void OnOffOptionPannel(bool _onoff)
    {
        _introCv.OnClickSetting(_onoff);
    }

    public void ShowMessageAlert(string msg)
    {
        _introCv.ShowMessageAlert(msg);
    }

    public void ShowAlert(AlertViewOptions avo)
    {
        _introCv.ShowAlert(avo);
    }

    //void InitCloth()
    //{
    //    _clothData = new Cloth()
    //    {
    //        _ShirtData = new List<ClothLockData>(),
    //        _PantsData = new List<ClothLockData>(),
    //        _DressData = new List<ClothLockData>(),
    //        _HatData = new List<ClothLockData>(),
    //        _EarringData = new List<ClothLockData>(),
    //        _GlassData = new List<ClothLockData>(),
    //        _ArmringData = new List<ClothLockData>(),
    //        _NecklessData = new List<ClothLockData>(),
    //        _ShoseData = new List<ClothLockData>(),
    //        _OtherData = new List<ClothLockData>(),
    //        _PetData = new List<ClothLockData>(),
    //        _AllBData = new List<ClothLockData>(),
    //        _AllAData = new List<ClothLockData>(),
    //        _BGData = new List<ClothLockData>()
    //    };
    //}

    //public Cloth OtherClothDataInit(Cloth _cloth)
    //{
    //    _cloth = new Cloth()
    //    {
    //        _ShirtData = new List<ClothLockData>(),
    //        _PantsData = new List<ClothLockData>(),
    //        _DressData = new List<ClothLockData>(),
    //        _HatData = new List<ClothLockData>(),
    //        _EarringData = new List<ClothLockData>(),
    //        _GlassData = new List<ClothLockData>(),
    //        _ArmringData = new List<ClothLockData>(),
    //        _NecklessData = new List<ClothLockData>(),
    //        _ShoseData = new List<ClothLockData>(),
    //        _OtherData = new List<ClothLockData>(),
    //        _PetData = new List<ClothLockData>(),
    //        _AllBData = new List<ClothLockData>(),
    //        _AllAData = new List<ClothLockData>(),
    //        _BGData = new List<ClothLockData>()
    //    };
    //    return _cloth;
    //}

    #region 화장 -> 옷입히기 씬 전환시 화장정보, 마스크 정보
    [SerializeField]
    private Texture _MaskTarget_Cheek;
    [SerializeField]
    private Texture _MaskTarget_Eyebrow;
    [SerializeField]
    private Texture _MaskTarget_Shadow;
    [SerializeField]
    private Texture _MaskTarget_Lipsitck;
    [SerializeField]
    private Sprite _HairTarget;
    [SerializeField]
    private Sprite _EyeTarget;
    [SerializeField]
    private int _skinIdx;

    /// <summary>
    /// 로딩 패널
    /// </summary>
    public GameObject _LoadingPannel;
    public void OnOffLoadPannel(bool _onoff)
    {
        AdsManager.Instance.ActiveBanner(false);
        _LoadingPannel.SetActive(_onoff);
    }

    /// <summary>
    /// 옷입히기 씬 전환시 옷입히기 씬에서 사용할 마스크 타겟을 화장씬의 마지막 단계에서 저장해둠
    /// </summary>
    /// <param name="_maskCheek"></param>
    /// <param name="_maskEyebrow"></param>
    /// <param name="_maskShadow"></param>
    /// <param name="_maskLipstick"></param>
    /// <param name="_hair"></param>
    /// <param name="_Eye"></param>
    public void LoadjujuSkin_OnClothes(Texture _maskCheek, Texture _maskEyebrow, Texture _maskShadow, Texture _maskLipstick, Sprite _hair, Sprite _Eye)
    {

        _MaskTarget_Cheek = _maskCheek;
        _MaskTarget_Eyebrow = _maskEyebrow;
        _MaskTarget_Shadow = _maskShadow;
        _MaskTarget_Lipsitck = _maskLipstick;

        _HairTarget = _hair;
        _EyeTarget = _Eye;


    }

    // 로드시 로드한 데이터를 월드매니저에 저장해두고 사용하는 
    public void SettingJujuData_InGame_Mask_Cheek(Texture2D _mask) => _MaskTarget_Cheek = _mask;
    public void SettingJujuData_InGame_Mask_EyeBrow(Texture2D _mask) => _MaskTarget_Eyebrow = _mask;
    public void SettingJujuData_InGame_Mask_Lipstick(Texture2D _mask) => _MaskTarget_Lipsitck = _mask;
    public void SettingJujuData_InGame_Mask_Shadow(Texture2D _mask) => _MaskTarget_Shadow = _mask;
    public void SettingJujuData_InGame_hair(Sprite _hair) => _HairTarget = _hair;
    public void SettingJujuData_InGame_Eye(Sprite _eye) => _EyeTarget = _eye;
    public void SettingJujuDate_InGame_Skin(int _skin) => _skinIdx = _skin;


    // 화장씬에서 보드 갯
    public void SetMaxToolIdx(int _idx) => _maxToolIdx = _idx;
    public int GetMaxToolIdx() { return _maxToolIdx; }

    Sprite[] fullSkinLoad;

    public void SetClothSceneJuju()
    {
        if (fullSkinLoad == null)
        {
            fullSkinLoad = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.FULLSKIN);
        }

        GirlHolder clothjuju = FindObjectOfType<GirlHolder>();
        if (clothjuju == null) { Debug.LogError("Not Found Girl Holder"); return; }
        if (_MaskTarget_Cheek == null) return;
        clothjuju._Mask_Cheek.texture = _MaskTarget_Cheek;
        clothjuju._Mask_EyeBrow.texture = _MaskTarget_Eyebrow;
        clothjuju._Mask_Lipstick.texture = _MaskTarget_Lipsitck;
        clothjuju._Mask_Shadow.texture = _MaskTarget_Shadow;
        clothjuju._Hair.sprite = _HairTarget;
        clothjuju._Eye.sprite = _EyeTarget;
        try
        {
            clothjuju._Skin.sprite = fullSkinLoad[SelectedJujuSkin];
        }
        catch
        {
            Debug.Log("MainScene Skin Load");
        }
    }
    #endregion

    // 아래는 옷 잠금 정보 관련
    //public Cloth GetClothData() { return _clothData; }
    //public void ResetClothData()
    //{
    //    _clothData = new Cloth();
    //}

    public bool GetHaveCloth(MenuSelector _cate, int _idx)
    {
        if (!GameManager.Instance.GameSettings.IsAdLive)
        {
            return true;
        }

        string key = string.Format("have_cloth_{0}_{1}", (int)_cate, _idx);
        int value = PlayerPrefs.GetInt(key, 0);
        return !DataSetup.Instance.IsLock(_cate, _idx) || value == 1;
    }

    public void SetHaveCloth(MenuSelector _cate, int _idx)
    {
        string key = string.Format("have_cloth_{0}_{1}", (int)_cate, _idx);
        PlayerPrefs.SetInt(key, 1);
    }

    // 잠금 상태 저장 (최초)
    //public void SetClothLockData_Init(MENUSELECTOR _cate, int _idx, bool _lock)
    //{
    //    switch (_cate)
    //    {
    //        case MENUSELECTOR.SHIRT:
    //            ClothLockData _data1 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._ShirtData.Add(_data1);
    //            break;
    //        case MENUSELECTOR.PANTS:
    //            ClothLockData _data2 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._PantsData.Add(_data2);
    //            break;
    //        case MENUSELECTOR.DRESS:
    //            ClothLockData _data3 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._DressData.Add(_data3);
    //            break;
    //        case MENUSELECTOR.HAT:
    //            ClothLockData _data4 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._HatData.Add(_data4);
    //            break;
    //        case MENUSELECTOR.EARING:
    //            ClothLockData _data5 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._EarringData.Add(_data5);
    //            break;
    //        case MENUSELECTOR.GLASS:
    //            ClothLockData _data6 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._GlassData.Add(_data6);
    //            break;
    //        case MENUSELECTOR.ARMRING:
    //            ClothLockData _data7 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._ArmringData.Add(_data7);
    //            break;
    //        case MENUSELECTOR.NECKLESS:
    //            ClothLockData _data8 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._NecklessData.Add(_data8);
    //            break;
    //        case MENUSELECTOR.SHOSE:
    //            ClothLockData _data9 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._ShoseData.Add(_data9);
    //            break;
    //        case MENUSELECTOR.OTHER:
    //            ClothLockData _data10 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._OtherData.Add(_data10);
    //            break;
    //        case MENUSELECTOR.PET:
    //            ClothLockData _data11 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._PetData.Add(_data11);
    //            break;
    //        case MENUSELECTOR.ALLINONEA:
    //            ClothLockData _data13 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._AllAData.Add(_data13);
    //            break;
    //        case MENUSELECTOR.ALLINONEB:
    //            ClothLockData _data12 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._AllBData.Add(_data12);
    //            break;
    //        case MENUSELECTOR.CLOTHBACKGROUND:
    //            ClothLockData _data14 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._AllAData.Add(_data14);
    //            break;
    //    }
    //}

    // 최초 이후 잠금 해제등 특정 동작에서 해당 리스트의 인덱스에 해당하는 데이터 덮어쓰기
    //public void SetClothLock(MENUSELECTOR _cate, int _idx, bool _lock)
    //{
    //    switch (_cate)
    //    {
    //        case MENUSELECTOR.SHIRT:
    //            ClothLockData _data1 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._ShirtData[_idx] = _data1;
    //            break;
    //        case MENUSELECTOR.PANTS:
    //            ClothLockData _data2 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._PantsData[_idx] = _data2;
    //            break;
    //        case MENUSELECTOR.DRESS:
    //            ClothLockData _data3 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._DressData[_idx] = _data3;
    //            break;
    //        case MENUSELECTOR.HAT:
    //            ClothLockData _data4 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._HatData[_idx] = _data4;
    //            break;
    //        case MENUSELECTOR.EARING:
    //            ClothLockData _data5 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._EarringData[_idx] = _data5;
    //            break;
    //        case MENUSELECTOR.GLASS:
    //            ClothLockData _data6 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._GlassData[_idx] = _data6;
    //            break;
    //        case MENUSELECTOR.ARMRING:
    //            ClothLockData _data7 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._ArmringData[_idx] = _data7;
    //            break;
    //        case MENUSELECTOR.NECKLESS:
    //            ClothLockData _data8 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._NecklessData[_idx] = _data8;
    //            break;
    //        case MENUSELECTOR.SHOSE:
    //            ClothLockData _data9 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._ShoseData[_idx] = _data9;
    //            break;
    //        case MENUSELECTOR.OTHER:
    //            ClothLockData _data10 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._OtherData[_idx] = _data10;
    //            break;
    //        case MENUSELECTOR.PET:
    //            ClothLockData _data11 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._PetData[_idx] = _data11;
    //            break;
    //        case MENUSELECTOR.ALLINONEA:
    //            ClothLockData _data13 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._AllAData[_idx] = _data13;
    //            break;
    //        case MENUSELECTOR.ALLINONEB:
    //            ClothLockData _data12 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._AllBData[_idx] = _data12;
    //            break;
    //        case MENUSELECTOR.CLOTHBACKGROUND:
    //            ClothLockData _data14 = new ClothLockData() { _iIdx = _idx, _isLock = _lock };
    //            _clothData._AllAData[_idx] = _data14;
    //            break;

    //    }

    //    string _clothstring = JsonUtility.ToJson(_clothData);
    //    Debug.Log(_clothstring);
    //    PlayerPrefs.SetString(_clothDataStr, _clothstring);
    //}
}
