using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;


/// <summary>
/// 세이브 로드 관련 기능 전담
/// 데이터 - 시리얼라이저블 클래스 => 제이슨(플레이어프렙스) / 마스크 및 스크린샷 : 이미지파일 (Persistant path)
/// </summary>
public class SaveLoadMgr : MonoBehaviour
{
    private static SaveLoadMgr _inst;
    public static SaveLoadMgr GetInst()
    {
        if (_inst != null) return _inst;
        else
        {
            Debug.LogError("SaveLoadmgr is Null"); return null;
        }
    }
    private void Awake()
    {
        if (_inst == null) _inst = this;
        DontDestroyOnLoad(this);
        _Slots = new List<GameObject>();
        flash.SetActive(false);
    }

    /// <summary>
    /// 씬이 로드 됬는지 체크
    /// </summary>
    [SerializeField]
    private bool _checkSceneLoaded = false;
    public bool SetCheckSceneLoad(bool _isSet) => _checkSceneLoaded = _isSet;
    public bool CheckSceneLoad() { return _checkSceneLoaded; }

    /// <summary>
    /// 저장 로드 슬롯 넘버 텍스트
    /// </summary>
    public Text _idxMainTxt;

    /// <summary>
    /// 슬롯 프리팹
    /// </summary>
    public GameObject _SlotPref;

    /// <summary>
    /// 저장 로드 패널에서 사용하는 하나하나의 슬롯
    /// </summary>
    [SerializeField]
    private List<GameObject> _Slots;
    [SerializeField]
    private GameObject _SelectedSlots;

    /// <summary>
    /// 슬롯 선택 인덱스
    /// </summary>
    private int _selectSlotIdx = 0;
    private int SaveSlotIdx
    {
        get { return _selectSlotIdx + (_slotIdx * 3) + 1; }
    }
    public GameObject _saveLoadPannel;
    public GameObject flash;
    public Texture _TransImg;

    public Sprite slotSelectImg;

    /// <summary>
    /// 세이브 / 로드 / 삭제 / 닫기 의 이미지
    /// </summary>
    public Image saveLoadTopObj;

    // 해당 스프라이트는 화장씬에서 SL패널을 비활성화함에 따라 필요없는 스프라이트
    public Sprite offSaveImg;
    public Sprite onSaveImg;


    Image[] _SaveInfo; // 옷 혹은 스킨 데이터 오브젝트

    // 각 슬롯에 할당하는 인덱스
    private int _slotIdx = 0;
    public int _SlotIdx
    {
        get { return _slotIdx; }
        set { _slotIdx = value; }
    }

    string _MaskPath = "";

    #region 눈, 머리, 옷(활성) 인덱스 저장용
    private int _eyeIdx;
    private int _hairIdx;
    private int _BGIdx;
    private int _SkinIdx;

    /// <summary>
    /// 이전 / 다음 버튼 관리 (예. 첫단계에서는 이전 버튼 비활성)
    /// </summary>
    [SerializeField]
    private Image[] _MakingStairImgs;

    /// <summary>
    /// 저장시 숨어야하는 UI 패널들 (아래로 사라졌다가 스크린샷 찍고 올라올때)
    /// </summary>
    public SlidePannel[] _SlidePan;

    public void SettingInfo_eye(int _idx) => _eyeIdx = _idx;
    public void SettingInfo_hair(int _idx) => _hairIdx = _idx;
    public void SettingInfo_Skin(int _idx) => _SkinIdx = _idx;
    public void SettingInfo_BG(int _idx) => _BGIdx = _idx;

    //public void SettingClothInfo(MENUSELECTOR _Selector, Dictionary<int, bool> _dic)=>
    //    _SavedDic_Cloth[_Selector] = new Dictionary<int, bool>(_dic);
    #endregion

    /// <summary>
    /// 최초 슬롯 3개 생성 / 이후 슬롯을 생성했었는지 판단
    /// </summary>
    bool _isAlreadyInstSlot = false;

    /// <summary>
    /// 스킨 선택 화면 :: 기존에 화장씬에서 저장하는 경우가 있었을때 사용하던 변수
    /// </summary>
    public GameObject _SelectSkingPannel;

    /// <summary>
    /// 저장로드 매니저 초기화
    /// </summary>
    public void SettingInit()
    {
        _idxMainTxt.text = (_slotIdx + 1).ToString();
        foreach (GameObject obj in _Slots)
        {
            obj.GetComponent<SlotController>().SelectSlot(false);
        }
        _Slots[_selectSlotIdx].GetComponent<SlotController>().SelectSlot(true);
        _SelectedSlots = _Slots[_selectSlotIdx];
    }

    /// <summary>
    /// 저장슬롯 클릭시
    /// </summary>
    /// <param name="_idx"></param>
    /// <param name="_idxOrigin"></param>
    public void OnClickSlot(int _idx, int _selectSlotIdx)
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        this._selectSlotIdx = _selectSlotIdx;
        _SelectedSlots = _Slots[_selectSlotIdx];
        //foreach (GameObject obj in _Slots)
        //{
        //    obj.GetComponent<SlotController>().SelectImg.enabled = false;
        //}
        //_Slots[_idxOrigin].GetComponent<SlotController>().SelectImg.enabled = true;
        //DotweenMgr.DoPopupOpen(.5f, 1f, .3f, _Slots[_idxOrigin].GetComponent<SlotController>().SelectImg.transform);
        foreach (GameObject obj in _Slots)
        {
            obj.GetComponent<SlotController>().SelectSlot(false);
        }
        _Slots[_selectSlotIdx].GetComponent<SlotController>().SelectSlot(true);
        //DotweenMgr.DoPopupOpen(.5f, 1f, .3f, _Slots[_idxOrigin].GetComponent<SlotController>().SelectImg.transform);
    }


    /// <summary>
    /// 저장로드 버튼 클릭시 :: 인트로 씬 / 화장씬에서 저장이 삭제됨 : Cloth경우만 보면됨
    /// </summary>
    public void OnClickSLPannel()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case GameManager.IntroSceneName:
                //saveLoadTopObj.sprite = offSaveImg;
                break;
            case GameManager.MainSceneName:
                //saveLoadTopObj.sprite = offSaveImg;
                if (!(_MakingStairImgs.Length > 0))
                {
                    MakingStairBtn _tempObj = FindObjectOfType<MakingStairBtn>();
                    _MakingStairImgs = _tempObj.GetComponentsInChildren<Image>();
                }
                foreach (Image _img in _MakingStairImgs)
                    _img.enabled = false;



                OHBtnChecker _ohbtn = FindObjectOfType<OHBtnChecker>();
                if (_ohbtn != null)
                {
                    MakingCanvas _make = FindObjectOfType<MakingCanvas>();
                    IntroCanvas _intro = FindObjectOfType<IntroCanvas>();
                    _intro.SetOhBtnObj(_ohbtn);
                    _make.OnOffBtn();
                    _ohbtn.gameObject.SetActive(false);
                }


                break;

            case GameManager.ClothSceneName:
                //saveLoadTopObj.sprite = onSaveImg;
                if (!(_MakingStairImgs.Length > 0))
                {
                    MakingStairBtn _tempObj = FindObjectOfType<MakingStairBtn>();
                    _MakingStairImgs = _tempObj.GetComponentsInChildren<Image>();
                }
                foreach (Image _img in _MakingStairImgs) _img.enabled = false;
                // 우측 사이드바 스크롤 열고 닫는 버튼
                OHBtnChecker _ohbtnCloth = FindObjectOfType<OHBtnChecker>();
                if (_ohbtnCloth != null)
                {
                    SideBarAnimator _sidebar = FindObjectOfType<SideBarAnimator>();
                    IntroCanvas _intro = FindObjectOfType<IntroCanvas>();
                    if (_sidebar == null)
                    {
                        _ohbtnCloth.gameObject.SetActive(false);
                        _intro.SetSLChecker(true);
                        _intro.SetChecker2(true);
                        break;
                    }
                    // 1208 임시주석
                    //_menuObj.OnClickOHBtn();
                    _ohbtnCloth.gameObject.SetActive(false);
                    _intro.SetSLChecker(true);
                }
                break;
        }

        if (!_isAlreadyInstSlot)
        {
            _isAlreadyInstSlot = true;
            for (int i = 0; i < 3; i++)
            {
                _Slots.Add(Instantiate<GameObject>(_SlotPref, _saveLoadPannel.GetComponentInChildren<HorizontalLayoutGroup>().transform));
            }
        }

        SettingInit();

        // SL 패널 열었을떄 슬롯 생성 및 세팅
        for (int i = 0; i < _Slots.Count; i++)
        {
            int nowSlotIdx = i + (_slotIdx * 3);
            _Slots[i].GetComponentInChildren<Text>().text = (nowSlotIdx + 1).ToString();

            int parseint;
            if (int.TryParse(_Slots[i].GetComponentInChildren<Text>().text, out parseint))
                Debug.Log("SuccessParse");
            else Debug.LogError("FailParse");
            int deleInt = i;
            _Slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            _Slots[i].GetComponent<Button>().onClick.AddListener(() => OnClickSlot(parseint, deleInt));

            string strSlotIdx = _Slots[i].GetComponentInChildren<Text>().text;
            string slotData = PlayerPrefs.GetString(strSlotIdx);
            if (!string.IsNullOrEmpty(slotData))
            {
                byte[] bytes = null;
                try
                {
                    bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "ScreenIdx" + strSlotIdx + ".png");
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Not Found Screen Shot");

                }
                if (bytes != null)
                {
                    Texture2D _tex = new Texture2D(1, 1);
                    _tex.LoadImage(bytes);

                    var tmp = RenderTexture.GetTemporary(_tex.width, _tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex, tmp);
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = tmp;
                    Texture2D mytex = new Texture2D(_tex.width, _tex.height);
                    mytex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                    mytex.Apply();
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(tmp);

                    _Slots[i].GetComponentInChildren<RawImage>().texture = mytex;
                }
            }
            else
            {
                _Slots[i].GetComponentInChildren<RawImage>().texture = _TransImg;
            }
        }
        Resources.UnloadUnusedAssets();
        //_isAlreadyInstSlot = true;
    }

    /// <summary>
    /// SL 패널에서 다음 버튼 클릭시 (슬롯마다 리스너 새로 할당, 이름에 맞는 스크린샷 로드)
    /// </summary>
    public void NextBtn_idx()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        ++_slotIdx;
        _idxMainTxt.text = (_slotIdx + 1).ToString();

        for (int i = 0; i < 3; i++)
        {
            int nowSlotIdx = i + (_slotIdx * 3);
            _Slots[i].GetComponentInChildren<Text>().text = (nowSlotIdx + 1).ToString();

            //string _temptxt = _Slots[i].GetComponentInChildren<Text>().text;
            //int tempint = 0;
            //if (int.TryParse(_temptxt, out tempint))
            //    _Slots[i].GetComponentInChildren<Text>().text = (tempint + 3).ToString();

            int parseint;
            if (int.TryParse(_Slots[i].GetComponentInChildren<Text>().text, out parseint))
                Debug.Log("SuccessParse");
            else Debug.LogError("FailParse");
            int deleInt = i;
            _Slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            _Slots[i].GetComponent<Button>().onClick.AddListener(() => OnClickSlot(parseint, deleInt));

            string strSlotIdx = _Slots[i].GetComponentInChildren<Text>().text;
            string slotData = PlayerPrefs.GetString(strSlotIdx);
            if (!string.IsNullOrEmpty(slotData))
            {
                byte[] bytes = null;
                try
                {
                    bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "ScreenIdx" + strSlotIdx + ".png");
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Not Found Screen Shot");

                }
                if (bytes != null)
                {
                    Texture2D _tex = new Texture2D(1, 1);
                    _tex.LoadImage(bytes);

                    var tmp = RenderTexture.GetTemporary(_tex.width, _tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex, tmp);
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = tmp;
                    Texture2D mytex = new Texture2D(_tex.width, _tex.height);
                    mytex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                    mytex.Apply();
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(tmp);

                    _Slots[i].GetComponentInChildren<RawImage>().texture = mytex;
                }
            }
            else
            {
                _Slots[i].GetComponentInChildren<RawImage>().texture = _TransImg;
            }
            Resources.UnloadUnusedAssets(); // 텍스쳐 생성하는 부분에서 메모리 누수가 있어서 이거 해줘야 함
        }
    }
    /// <summary>
    /// 이전 버튼 클릭시 (위 함수와 거의 동일한 기능 수행)
    /// </summary>
    public void PrevBtn_idx()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        if (_slotIdx <= 0) return;
        --_slotIdx;
        _idxMainTxt.text = (_slotIdx + 1).ToString();

        for (int i = 0; i < 3; i++)
        {
            int nowSlotIdx = i + (_slotIdx * 3);
            _Slots[i].GetComponentInChildren<Text>().text = (nowSlotIdx + 1).ToString();

            //string _temptxt = _Slots[i].GetComponentInChildren<Text>().text;
            //int tempint = 0;
            //if (int.TryParse(_temptxt, out tempint))
            //    _Slots[i].GetComponentInChildren<Text>().text = (tempint - 3).ToString();

            int parseint;
            if (int.TryParse(_Slots[i].GetComponentInChildren<Text>().text, out parseint))
                Debug.Log("SuccessParse");
            else Debug.LogError("FailParse");
            int deleInt = i;
            _Slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            _Slots[i].GetComponent<Button>().onClick.AddListener(() => OnClickSlot(parseint, deleInt));

            string strSlotIdx = _Slots[i].GetComponentInChildren<Text>().text;
            string slotData = PlayerPrefs.GetString(strSlotIdx);
            if (!string.IsNullOrEmpty(slotData))
            {
                byte[] bytes = null;
                try
                {
                    bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "ScreenIdx" + strSlotIdx + ".png");
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Not Found Screen Shot");

                }
                if (bytes != null)
                {
                    Texture2D _tex = new Texture2D(1, 1);
                    _tex.LoadImage(bytes);

                    var tmp = RenderTexture.GetTemporary(_tex.width, _tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex, tmp);
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = tmp;
                    Texture2D mytex = new Texture2D(_tex.width, _tex.height);
                    mytex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                    mytex.Apply();
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(tmp);

                    _Slots[i].GetComponentInChildren<RawImage>().texture = mytex;
                }
            }
            else
            {
                _Slots[i].GetComponentInChildren<RawImage>().texture = _TransImg;
            }
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 인트로씬 / 화장씬에서 저장로드 기능이 삭제되어 ClothScene일 경우에만 보면 됨
    /// </summary>
    /// <param name="_SceneIdx"></param>
    public void SaveXml(int _SceneIdx)
    {
        SoundMgr.GetInst().OnClickSoundBtn();


        if(_SelectedSlots.GetComponentInChildren<RawImage>().texture != _TransImg)
        {
            WorldMgr.GetInst().ShowAlert(new AlertViewOptions()
            {
                message = LocalizeManager.Instance.Localization("Save"),
                okButtonDelegate = () =>
                {
                    SaveSlot();
                }
            });
        }
        else
        {
            SaveSlot();
        }
    }

    private void SaveSlot()
    {
        Resources.UnloadUnusedAssets();
        if (SceneManager.GetActiveScene().name == GameManager.IntroSceneName || SceneManager.GetActiveScene().name == GameManager.MainSceneName)
        {
            Debug.LogWarning("Dont Save in IntroScene");
            return;
        }

        int _mainSceneIdx;

        SlotData _slot = new SlotData();
        if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
        {
            SwapToolBoard _toolboard = FindObjectOfType<SwapToolBoard>();
            _mainSceneIdx = _toolboard.ToolIdx;

            _slot = new SlotData()
            {
                _id = SaveSlotIdx,
                _sceneName = SceneManager.GetActiveScene().name,
                _MainSceneIdx = _mainSceneIdx,
                _slotskinIdx = _SkinIdx,
                _sloteyeIdx = _eyeIdx,
                _slothairIdx = _hairIdx,
                _slotBGIdx = _BGIdx,
                _cloth = new List<ClothData>()

            };
        }

        else
        {
            _slot = new SlotData()
            {
                _id = SaveSlotIdx,
                _sceneName = SceneManager.GetActiveScene().name,
                _slotskinIdx = _SkinIdx,
                _sloteyeIdx = _eyeIdx,
                _slothairIdx = _hairIdx,
                _slotBGIdx = _BGIdx,
                _cloth = new List<ClothData>()

            };
        }
        #region 마스크 로드
        FaceMaskChecker[] _mask = FindObjectsOfType<FaceMaskChecker>();
        foreach (FaceMaskChecker _tempObj in _mask)
        {
            switch (_tempObj.GetType().Name)
            {

                case "FaceMaskChecker_Cheek":

                    Debug.Log("Cheek Save!!");
                    Texture2D _maskTex1 = (Texture2D)_tempObj.gameObject.GetComponent<RawImage>().texture;
                    //string _path = "JujuMask";
                    // NativeGallery.SaveImageToGallery(_maskTex, _path, _SaveIdx.ToString() + ".png");

                    byte[] bytes = _maskTex1.EncodeToPNG();
                    var dirPath = Application.persistentDataPath + "/Resources";
                    if (!System.IO.Directory.Exists(dirPath))
                        System.IO.Directory.CreateDirectory(dirPath);
                    System.IO.File.WriteAllBytes(dirPath + "/Cheek" + SaveSlotIdx + ".png", bytes);
                    Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
                    break;

                case "FaceMaskChecker_Eyebrow":
                    Debug.Log("EyeBrow Save!!");
                    Texture2D _maskTex2 = (Texture2D)_tempObj.gameObject.GetComponent<RawImage>().texture;
                    //string _path = "JujuMask";
                    // NativeGallery.SaveImageToGallery(_maskTex, _path, _SaveIdx.ToString() + ".png");

                    byte[] bytes2 = _maskTex2.EncodeToPNG();
                    var dirPath2 = Application.persistentDataPath + "/Resources";
                    if (!System.IO.Directory.Exists(dirPath2))
                        System.IO.Directory.CreateDirectory(dirPath2);
                    System.IO.File.WriteAllBytes(dirPath2 + "/Eyebrow" + SaveSlotIdx + ".png", bytes2);
                    Debug.Log(bytes2.Length / 1024 + "Kb was saved as: " + dirPath2);
                    break;

                case "FaceMaskChecker_LipStick":
                    Debug.Log("Lipstick Save!!");
                    Texture2D _maskTex3 = (Texture2D)_tempObj.gameObject.GetComponent<RawImage>().texture;
                    //string _path = "JujuMask";
                    // NativeGallery.SaveImageToGallery(_maskTex, _path, _SaveIdx.ToString() + ".png");

                    byte[] bytes3 = _maskTex3.EncodeToPNG();
                    var dirPath3 = Application.persistentDataPath + "/Resources";
                    if (!System.IO.Directory.Exists(dirPath3))
                        System.IO.Directory.CreateDirectory(dirPath3);
                    System.IO.File.WriteAllBytes(dirPath3 + "/LipStick" + SaveSlotIdx + ".png", bytes3);
                    Debug.Log(bytes3.Length / 1024 + "Kb was saved as: " + dirPath3);
                    break;

                case "FaceMaskChecker_Shadow":
                    Debug.Log("Shadow Save!!");
                    Texture2D _maskTex4 = (Texture2D)_tempObj.gameObject.GetComponent<RawImage>().texture;
                    //string _path = "JujuMask";
                    // NativeGallery.SaveImageToGallery(_maskTex, _path, _SaveIdx.ToString() + ".png");

                    byte[] bytes4 = _maskTex4.EncodeToPNG();
                    var dirPath4 = Application.persistentDataPath + "/Resources";
                    if (!System.IO.Directory.Exists(dirPath4))
                        System.IO.Directory.CreateDirectory(dirPath4);
                    System.IO.File.WriteAllBytes(dirPath4 + "/Shadow" + SaveSlotIdx + ".png", bytes4);
                    Debug.Log(bytes4.Length / 1024 + "Kb was saved as: " + dirPath4);
                    break;


                default:
                    Debug.Log("Main Scene");
                    Texture2D _maskTex5 = (Texture2D)_tempObj.gameObject.GetComponent<RawImage>().texture;
                    //string _path = "JujuMask";
                    // NativeGallery.SaveImageToGallery(_maskTex, _path, _SaveIdx.ToString() + ".png");

                    byte[] bytes5 = _maskTex5.EncodeToPNG();
                    var dirPath5 = Application.persistentDataPath + "/Resources";
                    if (!System.IO.Directory.Exists(dirPath5))
                        System.IO.Directory.CreateDirectory(dirPath5);
                    System.IO.File.WriteAllBytes(dirPath5 + "/Making" + SaveSlotIdx + ".png", bytes5);
                    Debug.Log(bytes5.Length / 1024 + "Kb was saved as: " + dirPath5);
                    break;

            }
        }


        #endregion

        ClothData _tempcloth;
        DressHolder[] dressHolders = FindObjectsOfType<DressHolder>();
        for (int i = 0; i < dressHolders.Length; i++)
        {
            DressHolder dressHolder = dressHolders[i];
            _tempcloth = new ClothData()
            {
                _clothCate = dressHolder.ms,
                _cloth_resource_idx = dressHolder.Idx,
                _isEnable = dressHolder.IsShow
            };
            _slot._cloth.Add(_tempcloth);
        }

        string _jsonstr = JsonUtility.ToJson(_slot);
        PlayerPrefs.SetString(SaveSlotIdx.ToString(), _jsonstr);
        Debug.Log("Save The : " + _jsonstr);

        _SlidePan = FindObjectsOfType<SlidePannel>();

        foreach (SlidePannel _sl in _SlidePan)
            DotweenMgr.DoLocalMoveY(-FindObjectOfType<IntroCanvas>().GetComponent<RectTransform>().sizeDelta.y, .25f, _sl.transform);

        AdsManager.Instance.ActiveBanner(false);
        StartCoroutine(CaptureScreenshotAsTexture());
    }


    IEnumerator CaptureScreenshotAsTexture()
    {
        yield return new WaitForSeconds(3f);
        SoundMgr.GetInst().OnPlayOneShot("sfx19");

        if (flash != null)
        {
            flash.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            flash.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForEndOfFrame();

        Camera characterCamera = GameObject.FindGameObjectWithTag("CharacterCamera").GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(1080, 1920, 24);
        Texture2D screenShot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        characterCamera.targetTexture = rt;
        characterCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        screenShot.Apply();

        byte[] screenbytes = screenShot.EncodeToPNG();
        var screenpath = Application.persistentDataPath + "/Resources";
        if (!System.IO.Directory.Exists(screenpath))
            System.IO.Directory.CreateDirectory(screenpath);
        System.IO.File.WriteAllBytes(screenpath + "/ScreenIdx" + SaveSlotIdx + ".png", screenbytes);

        _Slots[_selectSlotIdx].GetComponentInChildren<RawImage>().texture = screenShot;
        RenderTexture.active = null;
        characterCamera.targetTexture = null;
        Destroy(rt);

        /*
        int slWidth = (int)Camera.main.pixelWidth / 3;
        int slHeight = (int)Camera.main.pixelHeight / 3;        
        Texture2D _screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D _screentex = ScaleTexture(_screenshot, slWidth, slHeight);
        byte[] screenbytes = _screentex.EncodeToPNG();
        var screenpath = Application.persistentDataPath + "/Resources";
        if (!System.IO.Directory.Exists(screenpath))
            System.IO.Directory.CreateDirectory(screenpath);
        System.IO.File.WriteAllBytes(screenpath + "/ScreenIdx" + SaveSlotIdx + ".png", screenbytes);

        _Slots[_selectSlotIdx].GetComponentInChildren<RawImage>().texture = _screentex;
        */

        Invoke("ReSetUp", .3f);
    }

    /// <summary>
    /// Slide패널들은 아래로 내리고 스크린샷 촬영하고 저장
    /// </summary>
    void SaveStart()
    {
        int slWidth = (int)Camera.main.pixelWidth / 3;
        int slHeight = (int)Camera.main.pixelHeight / 3;

        Texture2D _screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D _screentex = ScaleTexture(_screenshot, slWidth, slHeight);
        byte[] screenbytes = _screentex.EncodeToPNG();
        var screenpath = Application.persistentDataPath + "/Resources";
        if (!System.IO.Directory.Exists(screenpath))
            System.IO.Directory.CreateDirectory(screenpath);
        System.IO.File.WriteAllBytes(screenpath + "/ScreenIdx" + SaveSlotIdx + ".png", screenbytes);

        _Slots[_selectSlotIdx].GetComponentInChildren<RawImage>().texture = _screentex;

        Destroy(_screenshot);

        Invoke("ReSetUp", .3f);
    }

    void ReSetUp()
    {
        _SlidePan = FindObjectsOfType<SlidePannel>();
        foreach (SlidePannel _sl in _SlidePan)
            DotweenMgr.DoLocalMoveY(0f, .25f, _sl.transform);

        AdsManager.Instance.RequestBanner();
        AdsManager.Instance.WatchAdWithSave();

        Resources.UnloadUnusedAssets();
    }
 
    /// <summary>
    /// 스크린샷 사이즈 조정
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetWidth"></param>
    /// <param name="targetHeight"></param>
    /// <returns></returns>
    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    
    bool _isEqualSceneLoad = false;
    public void Set_isEqual(bool _check) => _isEqualSceneLoad = _check;
    IntroCanvas _introcanvas;

    /// <summary>
    /// 로드, 인트로/화장단계에서 저장이 막힘에 따라 ClothScene쪽만 보면 됨 
    /// </summary>
    public void LoadData()
    {
        string _jsonstr = PlayerPrefs.GetString(SaveSlotIdx.ToString());
        Debug.Log(_jsonstr);
        if (_jsonstr == "")
        {
            Debug.LogError("Slot Idx is Null");
            return;
        }

        SlotData _loadslot = JsonUtility.FromJson<SlotData>(_jsonstr);
        WorldMgr.GetInst().SetJujuSkin(_loadslot._slotskinIdx);
        if (_loadslot._sceneName == GameManager.MainSceneName)
        {
            if (!_isEqualSceneLoad)
            {
                _isEqualSceneLoad = true;
                WorldMgr.GetInst().OnOffLoadPannel(true);
                WorldMgr.GetInst().SetCheckLoadMainScene(true);
                WorldMgr.GetInst().SetMainSceneIdx(_loadslot._MainSceneIdx);
                
                SetCheckSceneLoad(true);
                SceneManager.LoadScene(GameManager.MainSceneName);
                return;
            }
        }

        if (SceneManager.GetActiveScene().name == GameManager.IntroSceneName)
        {
            WorldMgr.GetInst().OnOffLoadPannel(true);
            SetCheckSceneLoad(true);
            SceneManager.LoadScene(GameManager.MainSceneName);
            return;
        }
     
        if (_loadslot._sceneName != SceneManager.GetActiveScene().name)
        {
            WorldMgr.GetInst().OnOffLoadPannel(true);
            SetCheckSceneLoad(true);
           
            SceneManager.LoadScene(_loadslot._sceneName);
            return;
        }

        if (_SelectSkingPannel.activeInHierarchy)
        {
            _SelectSkingPannel.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
            MainSceneMgr.GetInst().LoadGirlHolderOnLoaded();
        if (_introcanvas == null) _introcanvas = FindObjectOfType<IntroCanvas>();
        _introcanvas.OnClickExitBtn_SL();

        WorldMgr.GetInst().OnOffOptionPannel(false);

        #region 마스크4가지 로드
        FaceMaskChecker[] _Allface = FindObjectsOfType<FaceMaskChecker>();
        foreach(FaceMaskChecker _face in _Allface)
        {
            switch (_face.GetType().Name)
            {
                case "FaceMaskChecker_Cheek":
                    byte[] bytes;
                    bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "Cheek" + SaveSlotIdx + ".png");

                    Texture2D _tex = new Texture2D(1, 1);
                    _tex.LoadImage(bytes);

                    var tmp = RenderTexture.GetTemporary(_tex.width, _tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex, tmp);
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = tmp;
                    Texture2D mytex = new Texture2D(_tex.width, _tex.height);
                    mytex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                    mytex.Apply();
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(tmp);

                    if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
                    {
                        _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().SetDrawingTexture(mytex);
                        _face.gameObject.GetComponent<RawImage>().texture = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().tex;
                        Debug.Log("Cheek MaskSet!!");
                    }
                    if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
                    {
                        _face.gameObject.GetComponent<RawImage>().texture = mytex;
                        WorldMgr.GetInst().SettingJujuData_InGame_Mask_Cheek(mytex); 
                        Debug.Log("Cheek MaskSet!!");
                    }
                    break;
                case "FaceMaskChecker_Eyebrow":

                    byte[] bytes2;
                    bytes2 = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "Eyebrow" + SaveSlotIdx + ".png");

                    Texture2D _tex2 = new Texture2D(1, 1);
                    _tex2.LoadImage(bytes2);

                    var tmp2 = RenderTexture.GetTemporary(_tex2.width, _tex2.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex2, tmp2);
                    RenderTexture previous2 = RenderTexture.active;
                    RenderTexture.active = tmp2;
                    Texture2D mytex2 = new Texture2D(_tex2.width, _tex2.height);
                    mytex2.ReadPixels(new Rect(0, 0, tmp2.width, tmp2.height), 0, 0);
                    mytex2.Apply();
                    RenderTexture.active = previous2;
                    RenderTexture.ReleaseTemporary(tmp2);

                    if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
                    {
                        _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().SetDrawingTexture(mytex2);
                        _face.gameObject.GetComponent<RawImage>().texture = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().tex;
                        Debug.Log("EyeBrwo MaskSet!!");
                    }
                    if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
                    {
                        _face.gameObject.GetComponent<RawImage>().texture = mytex2;
                        WorldMgr.GetInst().SettingJujuData_InGame_Mask_EyeBrow(mytex2);
                        Debug.Log("EyeBrwo MaskSet!!");
                    }
                    break;
                case "FaceMaskChecker_LipStick":
                    byte[] bytes3;
                    bytes3 = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "LipStick" + SaveSlotIdx + ".png");

                    Texture2D _tex3 = new Texture2D(1, 1);
                    _tex3.LoadImage(bytes3);

                    var tmp3 = RenderTexture.GetTemporary(_tex3.width, _tex3.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex3, tmp3);
                    RenderTexture previous3 = RenderTexture.active;
                    RenderTexture.active = tmp3;
                    Texture2D mytex3 = new Texture2D(_tex3.width, _tex3.height);
                    mytex3.ReadPixels(new Rect(0, 0, tmp3.width, tmp3.height), 0, 0);
                    mytex3.Apply();
                    RenderTexture.active = previous3;
                    RenderTexture.ReleaseTemporary(tmp3);

                    if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
                    {
                        _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().SetDrawingTexture(mytex3);
                        _face.gameObject.GetComponent<RawImage>().texture = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().tex;
                        Debug.Log("LipStickMaskSet!!");
                    }
                    if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
                    {
                        _face.gameObject.GetComponent<RawImage>().texture = mytex3;
                        WorldMgr.GetInst().SettingJujuData_InGame_Mask_Lipstick(mytex3);
                        Debug.Log("LipStickMaskSet!!");
                    }
                    break;
                case "FaceMaskChecker_Shadow":
                    byte[] bytes4;
                    bytes4 = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "Shadow" + SaveSlotIdx + ".png");

                    Texture2D _tex4 = new Texture2D(1, 1);
                    _tex4.LoadImage(bytes4);

                    var tmp4 = RenderTexture.GetTemporary(_tex4.width, _tex4.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex4, tmp4);
                    RenderTexture previous4 = RenderTexture.active;
                    RenderTexture.active = tmp4;
                    Texture2D mytex4 = new Texture2D(_tex4.width, _tex4.height);
                    mytex4.ReadPixels(new Rect(0, 0, tmp4.width, tmp4.height), 0, 0);
                    mytex4.Apply();
                    RenderTexture.active = previous4;
                    RenderTexture.ReleaseTemporary(tmp4);

                    if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
                    {
                        _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().SetDrawingTexture(mytex4);
                        _face.gameObject.GetComponent<RawImage>().texture = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().tex;
                        Debug.Log("MShadowaskSet!!");
                    }
                    if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
                    {
                        _face.gameObject.GetComponent<RawImage>().texture = mytex4;
                        WorldMgr.GetInst().SettingJujuData_InGame_Mask_Shadow(mytex4);
                        Debug.Log("Shadow MaskSet!!");
                    }
                    break;
                default:
                    byte[] bytes5;
                    bytes5 = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/Resources/" + "Making" + SaveSlotIdx + ".png");

                    Texture2D _tex5 = new Texture2D(1, 1);
                    _tex5.LoadImage(bytes5);

                    var tmp5 = RenderTexture.GetTemporary(_tex5.width, _tex5.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(_tex5, tmp5);
                    RenderTexture previous5 = RenderTexture.active;
                    RenderTexture.active = tmp5;
                    Texture2D mytex5 = new Texture2D(_tex5.width, _tex5.height);
                    mytex5.ReadPixels(new Rect(0, 0, tmp5.width, tmp5.height), 0, 0);
                    mytex5.Apply();
                    RenderTexture.active = previous5;
                    RenderTexture.ReleaseTemporary(tmp5);

                    if (SceneManager.GetActiveScene().name == GameManager.MainSceneName)
                    {
                        _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().SetDrawingTexture(mytex5);
                        _face.gameObject.GetComponent<RawImage>().texture = _face.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().tex;
                        Debug.Log("MShadowaskSet!!");
                    }
                 
                    break;
            }
        }

        #endregion

        DressHolder[] dressHolders = FindObjectsOfType<DressHolder>();
        for (int i = 0; i < dressHolders.Length; i++)
        {
            DressHolder dressHolder = dressHolders[i];
            ClothData _data = _loadslot._cloth.Find(x => x._clothCate == dressHolder.ms);
            if (_data != null)
            {
                dressHolder.SetIdx(_data._cloth_resource_idx);
                if (_data._isEnable)
                {
                    dressHolder.Show();
                }
                else
                {
                    dressHolder.Hide();
                }
            }
            else
            {
                dressHolder.Hide();
            }
        }

        SettingInfo_Skin(_loadslot._slotskinIdx);
        WorldMgr.GetInst().SettingJujuDate_InGame_Skin(_loadslot._slotskinIdx);

        SettingInfo_hair(_loadslot._slothairIdx);
        WorldMgr.GetInst().SettingJujuData_InGame_hair(MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.HAIR)[_loadslot._slothairIdx]);

        SettingInfo_eye(_loadslot._sloteyeIdx);
        WorldMgr.GetInst().SettingJujuData_InGame_Eye(MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.EYES)[_loadslot._sloteyeIdx]);

        WorldMgr.GetInst().SetClothSceneJuju();
        Resources.UnloadUnusedAssets();

        ClothEvent.ClothLoaded();
    }

    /// <summary>
    /// 해당 슬롯의 데이터 삭제 
    /// </summary>
    public void OnClickDeleteBtn()
    {
        SoundMgr.GetInst().OnClickSoundBtn();

        WorldMgr.GetInst().ShowAlert(new AlertViewOptions()
        {
            message = LocalizeManager.Instance.Localization("Delete"),
            okButtonDelegate = () =>
            {
                try
                {
                    File.Delete(Application.persistentDataPath + "/Resources/" + "Cheek" + SaveSlotIdx + ".png");
                    File.Delete(Application.persistentDataPath + "/Resources/" + "Eyebrow" + SaveSlotIdx + ".png");
                    File.Delete(Application.persistentDataPath + "/Resources/" + "LipStick" + SaveSlotIdx + ".png");
                    File.Delete(Application.persistentDataPath + "/Resources/" + "Shadow" + SaveSlotIdx + ".png");
                    File.Delete(Application.persistentDataPath + "/Resources/" + "ScreenIdx" + SaveSlotIdx + ".png");
                    PlayerPrefs.DeleteKey(SaveSlotIdx.ToString());
                    _Slots[_selectSlotIdx].GetComponentInChildren<RawImage>().texture = _TransImg;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Not Found Delete File");
                }
            }
        });

    }

    //public void OnClickDeleteBtn()
    //{
    //    SoundMgr.GetInst().OnClickSoundBtn();
    //    try
    //    {
    //        File.Delete(Application.persistentDataPath + "/Resources/" + "Cheek" + SaveSlotIdx + ".png");
    //        File.Delete(Application.persistentDataPath + "/Resources/" + "Eyebrow" + SaveSlotIdx + ".png");
    //        File.Delete(Application.persistentDataPath + "/Resources/" + "LipStick" + SaveSlotIdx + ".png");
    //        File.Delete(Application.persistentDataPath + "/Resources/" + "Shadow" + SaveSlotIdx + ".png");
    //        File.Delete(Application.persistentDataPath + "/Resources/" + "ScreenIdx" + SaveSlotIdx + ".png");
    //        PlayerPrefs.DeleteKey(SaveSlotIdx.ToString());
    //        _Slots[_selectSlotIdx].GetComponentInChildren<RawImage>().texture = _TransImg;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogWarning("Not Found Delete File");
    //    }

    //}
}
