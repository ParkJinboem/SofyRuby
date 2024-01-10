using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum LENSCOLOR
{
    RED = 0,
    YELLOW,
    GREEN,
    BLUE,
    MAGENTA,
    GREY,
    BROWN
}
/// <summary>
/// 사이드바가 나와야 하는 툴들의 기능 관리
/// </summary>
public class LensTool : MonoBehaviour
{
    #region Sprites
    private Sprite[] _LensSprites;      // 착용 렌즈 이미지
    private Sprite[] _EyeSprites;       // 렌즈 컬러 선택


   private Sprite[] _EyeShadow;         // 아이섀도 마스크
   private Sprite[] _EyeShadowColor;    // 아이섀도 컬러 선택


    private Sprite[] _LipStick;         // 립스틱 마스크
    private Sprite[] _LipStickColor;    // 립스틱 컬러 선택

    private Sprite[] _Cheek;
    private Sprite[] _CheekColor;

    private Sprite[] _Eyebrow;
    private Sprite[] _EyebrowColor;

    private Sprite[] _tools;            // 도구 모음
    #endregion

    public Image _EyeObj;
    public Image _CloseEyeObj;

    RectTransform rectTransform;
    ScrollRect _test;
    public GameObject _ColorObjPref;
    public GameObject _LipStickPref;
    public GameObject _EyeBrushPref;


    public GameObject _CheekBrushPref;
    public GameObject _EyeBrowBrushPref;

    /// <summary>
    /// 인스턴스 (폐기 코드 복구시 사용)
    /// </summary>
    private GameObject _CheekBrush;
    private GameObject _EyeBrowBrush;


    private GameObject _lipstickObj_ins;
    private GameObject _eyebrushObj_ins;

    private bool _LensToolSelect = false;

    string Tagstr_Lens = "Lens";
    string Tagstr_EyeShadow = "Shadow";
    string Tagstr_Lipstick = "Lipstick";
    string Tagstr_Cheek = "Cheek";
    string Tagstr_Eyebrow = "Eyebrow";

    #region 마스크 타겟 // 콜라이더
    public AdvancedMobilePaint.AdvancedMobilePaint _TargetMask_Cheek; // 기존에 사용하던 마스크 타겟
    public AdvancedMobilePaint.AdvancedMobilePaint _TargetMask_EyeBrow;
    public AdvancedMobilePaint.AdvancedMobilePaint _TargetMask_Shadow;
    public AdvancedMobilePaint.AdvancedMobilePaint _TargetMask_Lipstick;
    #endregion

    [SerializeField] int cheekDrawSize = 24;
    [SerializeField] int eyeBrowDrawSize = 9;
    [SerializeField] int shadowDrawSize = 12;
    [SerializeField] int lipstickDrawSize = 16;

    private MakingCanvas _MakingCanvas;


    public GameObject _OHBtn;

    Vector2 _rtr; // 치크 연관 recttr용
     Vector2 _originalPos_EyeBrowBrush;
    public bool _isPickUpBrush = false;


    public ParticleSystem[] _EyeParticle;
    

    List<ColorButton> LensColorList = new List<ColorButton>();
    List<ColorButton> ShadowColorList = new List<ColorButton>();
    List<ColorButton> LipstickColorList = new List<ColorButton>();
    List<ColorButton> EyebrowColorList = new List<ColorButton>();
    List<ColorButton> CheekColorList = new List<ColorButton>();

    public RectTransform instanceTransform;

    private void Start()
    {
        _tools = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.TOOL);

        _MakingCanvas = FindObjectOfType<MakingCanvas>();
        rectTransform = GetComponent<RectTransform>();
        _test = GetComponent<ScrollRect>();
        _originalPos_EyeBrowBrush = new Vector2(890f, 280f);
        Init1();
    }
    public void Init1()
    {
        _LensSprites = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.LENS);
    

        _EyeShadowColor = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.EYESHADOWCOLOR);
        Invoke("Init2", 1f);
    }
    public void Init2()
    {
        _LipStickColor = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.LIPSTICKCOLOR);

        _CheekColor = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.CHEEKCOLOR);
        Invoke("Init3", 1f);
    }
    public void Init3()
    {
        _EyebrowColor = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.EYEBROWCOLOR);
    }

 
    /// <summary>
    /// 이미 이전에 누른적 있는지 체크 => 누른적 있으면 엑티브 상태 변경하고 리턴 / 없으면 새로 생성
    /// </summary>
    public void OnClickLensTool(CATEGORY _idx)
    {
      
       // SoundMgr.GetInst().OnClickSoundBtn();
        GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        //SoundMgr.GetInst().OnClickSoundBtn();
        if (_LensToolSelect) return;
        if (!_OHBtn.activeInHierarchy)
        {
            _OHBtn.SetActive(true);
            DotweenMgr.DoPopupOpen(0f, 1f, .3f, _OHBtn.transform);
        }
        //  _isPickUpBrush = true;
        //SoundMgr.GetInst().OnClickSoundBtn();
        bool isAlreadyIns = false;
        foreach(RectTransform _obj in _test.content)
        {
            switch (_idx)
            {
                case CATEGORY.LENS:
                    if (_obj.CompareTag(Tagstr_Lens)) isAlreadyIns = true;
                    break;
                case CATEGORY.SHADOW:
                    if (_obj.CompareTag(Tagstr_EyeShadow)) isAlreadyIns = true;
                    break;
                case CATEGORY.LIPSTICK:
                    if (_obj.CompareTag(Tagstr_Lipstick)) isAlreadyIns = true;
                    break;
                case CATEGORY.CHEEK:
                    if (_obj.CompareTag(Tagstr_Cheek)) isAlreadyIns = true;
                    break;
                case CATEGORY.EYEBROW:
                    if (_obj.CompareTag(Tagstr_Eyebrow)) isAlreadyIns = true;
                    break;
            }
            if (isAlreadyIns) break;
        }
        if (isAlreadyIns)
        {
            switch (_idx)
            {
                case CATEGORY.LENS: AlreadyInsToll(Tagstr_Lens);
                    break;
                case CATEGORY.SHADOW:
                    _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = true;
                    _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;

                    _TargetMask_Shadow.enabled = true;
                    _TargetMask_EyeBrow.enabled = false;
                    _TargetMask_Lipstick.enabled = false;
                    _TargetMask_Cheek.enabled = false;

                    _TargetMask_Shadow.SetDrawPos(new Vector3(-30f, 230f));
                    _TargetMask_Shadow.SetDrawingTexture((Texture2D)_TargetMask_Shadow.gameObject.GetComponent<RawImage>().texture);
                    _TargetMask_Shadow.gameObject.GetComponent<RawImage>().texture = _TargetMask_Shadow.tex;
                    AlreadyInsToll(Tagstr_EyeShadow);
                    break;
                case CATEGORY.LIPSTICK:
                    _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = true;

                    _TargetMask_Shadow.enabled = false;
                    _TargetMask_EyeBrow.enabled = false;
                    _TargetMask_Lipstick.enabled = true;
                    _TargetMask_Cheek.enabled = false;

                    _TargetMask_Lipstick.SetDrawPos(new Vector3(0f, 320f));
                    _TargetMask_Lipstick.SetDrawingTexture((Texture2D)_TargetMask_Lipstick.gameObject.GetComponent<RawImage>().texture);
                    _TargetMask_Lipstick.gameObject.GetComponent<RawImage>().texture = _TargetMask_Lipstick.tex;
                    AlreadyInsToll(Tagstr_Lipstick);
                    break;
                case CATEGORY.CHEEK:
                    _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = true;
                    _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;

                    _TargetMask_Shadow.enabled = false;
                    _TargetMask_EyeBrow.enabled = false;
                    _TargetMask_Lipstick.enabled = false;
                    _TargetMask_Cheek.enabled = true;

                    _TargetMask_Cheek.SetDrawPos(new Vector3(0f, 280f));
                    _TargetMask_Cheek.SetDrawingTexture((Texture2D)_TargetMask_Cheek.gameObject.GetComponent<RawImage>().texture);

                    _TargetMask_Cheek.gameObject.GetComponent<RawImage>().texture = _TargetMask_Cheek.tex;
                    AlreadyInsToll(Tagstr_Cheek);
                    break;
                case CATEGORY.EYEBROW:
                    _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = true;
                    _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;
                    _TargetMask_Shadow.enabled = false;
                    _TargetMask_EyeBrow.enabled = true;
                    _TargetMask_Lipstick.enabled = false;
                    _TargetMask_Cheek.enabled = false;


                    _TargetMask_EyeBrow.SetDrawPos(new Vector3(0f, 350f));
                    _TargetMask_EyeBrow.SetDrawingTexture((Texture2D)_TargetMask_EyeBrow.gameObject.GetComponent<RawImage>().texture);

                    _TargetMask_EyeBrow.gameObject.GetComponent<RawImage>().texture = _TargetMask_EyeBrow.tex;
                    AlreadyInsToll(Tagstr_Eyebrow);
                    break;
            }
            _LensToolSelect = true;
            return;
        }
        switch (_idx)
        {
            case CATEGORY.LENS:
                if(_EyeSprites == null)
                {
                    _EyeSprites = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.EYES);
                }
                LensColorList.Clear();
                for (int i = 0; i < _LensSprites.Length; i++)
                {
                    int testint = i;
                    GameObject _te = Instantiate(_ColorObjPref, _test.content);
                    _te.tag = Tagstr_Lens;
                    ColorButton tempBtn = _te.GetComponent<ColorButton>();
                    LensColorList.Add(tempBtn);
                    tempBtn.imgIcon.sprite =_LensSprites[i];
                    //_te.GetComponent<Image>().sprite =
                    _te.GetComponent<Button>().onClick.RemoveAllListeners();
                    _te.GetComponent<Button>().onClick.AddListener(()=>{ OnClickMethod_Lens(testint, tempBtn); });
             
                }
                _LensToolSelect = true;
                break;
                
            case CATEGORY.SHADOW:
                if(_EyeShadow == null)
                {
                    _EyeShadow = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.EYESHADOW);
                }
                _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = true;
                _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;

                _TargetMask_Shadow.enabled = true;
                _TargetMask_EyeBrow.enabled = false;
                _TargetMask_Lipstick.enabled = false;
                _TargetMask_Cheek.enabled = false;


                _TargetMask_Shadow.SetDrawPos(new Vector3(-30f, 230f));

                _TargetMask_Shadow.SetDrawingTexture((Texture2D)_TargetMask_Shadow.gameObject.GetComponent<RawImage>().texture);
                _TargetMask_Shadow.gameObject.GetComponent<RawImage>().texture = _TargetMask_Shadow.tex;
                for (int i = 0; i < _EyeShadowColor.Length; i++)
                {
                    int testint = i;
                    GameObject _te = Instantiate(_ColorObjPref, _test.content);
                    ColorButton tempBtn = _te.GetComponent<ColorButton>();
                    ShadowColorList.Add(tempBtn);
                    tempBtn.imgIcon.sprite = _EyeShadowColor[i];
                    tempBtn.GetComponent<Button>().enabled = false;
                    _te.AddComponent<EventTrigger>();
                    _te.tag = Tagstr_EyeShadow;
                    //Button button = _te.GetComponent<Button>();
                    //tempBtn.SettingInt(i, null);
             

                    #region EventTriggerCallBack

                    EventTrigger.Entry entry_Ev = new EventTrigger.Entry();

                    entry_Ev.eventID = EventTriggerType.PointerDown;
                    entry_Ev.callback.AddListener((data) => { OnPointDown_EyeBrush((PointerEventData)data, testint, tempBtn); });
                    _te.GetComponent<EventTrigger>().triggers.Add(entry_Ev);
                    if (i == 0) continue;

                    EventTrigger.Entry _entry_Ev_Drag = new EventTrigger.Entry();
                    _entry_Ev_Drag.eventID = EventTriggerType.Drag;
                    _entry_Ev_Drag.callback.AddListener((data) => { OnDrag_EyeBrush((PointerEventData)data, tempBtn); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_Drag);

                    EventTrigger.Entry _entry_Ev_PTUP = new EventTrigger.Entry();
                    _entry_Ev_PTUP.eventID = EventTriggerType.PointerUp;
                    _entry_Ev_PTUP.callback.AddListener((data) => { OnPointUp_EyeBrush((PointerEventData)data, tempBtn); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_PTUP);

                    #endregion

                    #region Unity Event Tools => 원하던게 아님
                    //UnityAction<int, colorbu> unityaction = OnPointDown_EyeBrush;
                    //UnityEventTools.AddIntPersistentListener(_te.GetComponent<PrefBtn_EventFunc>().Get_ev(), unityaction, testint);
                    #endregion


                }
                _LensToolSelect = true;
                break;
            case CATEGORY.LIPSTICK:
                if(_LipStick == null)
                {
                    _LipStick = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.LIPSTICK);
                }
                _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = true;

                _TargetMask_Shadow.enabled = false;
                _TargetMask_EyeBrow.enabled = false;
                _TargetMask_Lipstick.enabled = true;
                _TargetMask_Cheek.enabled = false;

                _TargetMask_Lipstick.SetDrawPos(new Vector3(0f, 340f));
                _TargetMask_Lipstick.SetDrawingTexture((Texture2D)_TargetMask_Lipstick.gameObject.GetComponent<RawImage>().texture);
                _TargetMask_Lipstick.gameObject.GetComponent<RawImage>().texture = _TargetMask_Lipstick.tex;

                for (int i = 0; i < _LipStickColor.Length; i++)
                {
                    int testint = i;
                    GameObject _te = Instantiate(_ColorObjPref, _test.content);
                    ColorButton tempBtn = _te.GetComponent<ColorButton>();
                    LipstickColorList.Add(tempBtn);
                    tempBtn.imgIcon.sprite = _LipStickColor[i];
                    _te.GetComponent<Button>().enabled = false;
                    _te.AddComponent<EventTrigger>();
                    _te.tag = Tagstr_Lipstick;

               
                    #region EventTriggerCallBack
                    EventTrigger.Entry entry_Ev = new EventTrigger.Entry();
                    entry_Ev.eventID = EventTriggerType.PointerDown;
                    entry_Ev.callback.AddListener((data) => { OnPointDown_LipStick((PointerEventData)data, testint, tempBtn); });
                    _te.GetComponent<EventTrigger>().triggers.Add(entry_Ev);
                    if (i == 0) continue;
                    EventTrigger.Entry _entry_Ev_Drag = new EventTrigger.Entry();
                    _entry_Ev_Drag.eventID = EventTriggerType.Drag;
                    _entry_Ev_Drag.callback.AddListener((data) => { OnDrag_LipStick((PointerEventData)data); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_Drag);

                    EventTrigger.Entry _entry_Ev_PTUP = new EventTrigger.Entry();
                    _entry_Ev_PTUP.eventID = EventTriggerType.PointerUp;
                    _entry_Ev_PTUP.callback.AddListener((data) => { OnPointUp_LipStick((PointerEventData)data); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_PTUP);
                    #endregion

         
                }
                _LensToolSelect = true;
                break;

            case CATEGORY.CHEEK:
                if(_Cheek == null)
                {

                    _Cheek = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.CHEEK);
                }
                _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = true;
                _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;

                _TargetMask_Shadow.enabled = false;
                _TargetMask_EyeBrow.enabled = false;
                _TargetMask_Lipstick.enabled = false;
                _TargetMask_Cheek.enabled = true;

                _TargetMask_Cheek.SetDrawPos(new Vector3(0f, 280f));
                _TargetMask_Cheek.SetDrawingTexture((Texture2D)_TargetMask_Cheek.gameObject.GetComponent<RawImage>().texture);

                _TargetMask_Cheek.gameObject.GetComponent<RawImage>().texture = _TargetMask_Cheek.tex;

                for (int i = 0; i < _CheekColor.Length; i++)
                {
                    int testint = i;
                    GameObject _te = Instantiate(_ColorObjPref, _test.content);
                    
                    ColorButton tempBtn = _te.GetComponent<ColorButton>();
                    CheekColorList.Add(tempBtn);
                    tempBtn.imgIcon.sprite = _CheekColor[i];
                    _te.GetComponent<Button>().enabled = false;
                    _te.AddComponent<EventTrigger>();
                    _te.tag = Tagstr_Cheek;
                 
                    #region EventTriggerCallBack
                    EventTrigger.Entry entry_Ev = new EventTrigger.Entry();
                    entry_Ev.eventID = EventTriggerType.PointerDown;
                    entry_Ev.callback.AddListener((data) => { OnPointDown_Cheek((PointerEventData)data, testint, tempBtn); });
                    _te.GetComponent<EventTrigger>().triggers.Add(entry_Ev);
                    if (i == 0) continue;
                    EventTrigger.Entry _entry_Ev_Drag = new EventTrigger.Entry();
                    _entry_Ev_Drag.eventID = EventTriggerType.Drag;
                    _entry_Ev_Drag.callback.AddListener((data) => { OnDrag_Cheek((PointerEventData)data); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_Drag);

                    EventTrigger.Entry _entry_Ev_PTUP = new EventTrigger.Entry();
                    _entry_Ev_PTUP.eventID = EventTriggerType.PointerUp;
                    _entry_Ev_PTUP.callback.AddListener((data) => { OnPointUp_Cheek((PointerEventData)data); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_PTUP);
                    #endregion

             
                }
                _LensToolSelect = true;
                break;
            case CATEGORY.EYEBROW:
                if(_Eyebrow == null)
                {
                    _Eyebrow = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.EYEBROW);
                }
                _TargetMask_Cheek.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Shadow.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_EyeBrow.gameObject.GetComponent<MeshCollider>().enabled = true;
                _TargetMask_Lipstick.gameObject.GetComponent<MeshCollider>().enabled = false;
                _TargetMask_Shadow.enabled = false;
                _TargetMask_EyeBrow.enabled = true;
                _TargetMask_Lipstick.enabled = false;
                _TargetMask_Cheek.enabled = false;


                _TargetMask_EyeBrow.SetDrawPos(new Vector3(0f, 280f));
                _TargetMask_EyeBrow.SetDrawingTexture((Texture2D)_TargetMask_EyeBrow.gameObject.GetComponent<RawImage>().texture);

                _TargetMask_EyeBrow.gameObject.GetComponent<RawImage>().texture = _TargetMask_EyeBrow.tex;

                for (int i = 0; i < _EyebrowColor.Length; i++)
                {
                    int testint = i;
                    GameObject _te = Instantiate(_ColorObjPref, _test.content);
                    ColorButton tempBtn = _te.GetComponent<ColorButton>();
                    EyebrowColorList.Add(tempBtn);
                    tempBtn.imgIcon.sprite = _EyebrowColor[i];
                    _te.GetComponent<Button>().enabled = false;
                    _te.AddComponent<EventTrigger>();
                    _te.tag = Tagstr_Eyebrow;

                    

                    #region EventTriggerCallBack
                    EventTrigger.Entry entry_Ev = new EventTrigger.Entry();
                    entry_Ev.eventID = EventTriggerType.PointerDown;
                    entry_Ev.callback.AddListener((data) => { OnPointDown_EyeBrow((PointerEventData)data, testint, tempBtn); });
                    _te.GetComponent<EventTrigger>().triggers.Add(entry_Ev);
                    if (i == 0) continue;
                    EventTrigger.Entry _entry_Ev_Drag = new EventTrigger.Entry();
                    _entry_Ev_Drag.eventID = EventTriggerType.Drag;
                    _entry_Ev_Drag.callback.AddListener((data) => { OnDrag_EyeBrow((PointerEventData)data); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_Drag);

                    EventTrigger.Entry _entry_Ev_PTUP = new EventTrigger.Entry();
                    _entry_Ev_PTUP.eventID = EventTriggerType.PointerUp;
                    _entry_Ev_PTUP.callback.AddListener((data) => { OnPointUp_EyeBrow((PointerEventData)data); });
                    _te.GetComponent<EventTrigger>().triggers.Add(_entry_Ev_PTUP);
                    #endregion

                 
                }

                _LensToolSelect = true;
                break;
        }
       
    }
    private void AlreadyInsToll(string _tag)
    {
        for (int i = 0; i < _test.content.childCount; i++)
        {
            if (_test.content.GetChild(i).gameObject.CompareTag(_tag))
                _test.content.GetChild(i).gameObject.SetActive(true);
        }
    }
    

    void OnPointerDown(PointerEventData data)
    {
        Debug.Log("Pointer Down");
    }
    void OnClickMethod_Lens(int _idx, ColorButton _selectButton)
    {
        foreach(ParticleSystem _part in _EyeParticle)
        {
            _part.Play();
        }
        foreach(ColorButton prefs in LensColorList)
        {
            prefs.imgSelected.enabled = false;
        }
        if (_idx > 0)
        {
            _selectButton.imgSelected.enabled = true;
            DotweenMgr.DoPopupOpen(0f, 1f, .3f, _selectButton.imgSelected.transform);
        }
        SoundMgr.GetInst().OnPlayOneShot("se_01");
        _EyeObj.sprite = _EyeSprites[_idx];
        SaveLoadMgr.GetInst().SettingInfo_eye(_idx);
    }

    void selectImgEnable(List<ColorButton> buttons)
    {
        foreach (ColorButton obj in buttons)
        {
            if (obj.imgSelected.enabled)
                obj.imgSelected.enabled = false;
        }
    }
  
    /// <summary>
    /// 동적으로 추가되는 이벤트 트리거 포인트다운
    /// 해야될것 : 페인트툴 패턴 마스크 교체, 브러쉬 생성(생성된게 있다면 액티브), 페인트툴 드로우 활성화
    /// </summary>
    /// <param name="data">이벤트데이터</param>
    /// <param name="_idx">컬러 인덱스</param>
    void OnPointDown_EyeBrush(PointerEventData data,int _idx, ColorButton selectButton) // 아이섀도
    {
        _test.OnBeginDrag(data);
        if (_MakingCanvas == null) Debug.LogError("Canvas NULL");
        if (_idx > 0)
        {
            if (_eyebrushObj_ins == null)
                _eyebrushObj_ins = Instantiate(_EyeBrushPref, instanceTransform);
            _eyebrushObj_ins.SetActive(true);
            //_eyebrushObj_ins.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000f);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            _eyebrushObj_ins.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

            SoundMgr.GetInst().OnClickSoundBtn();
            _EyeObj.gameObject.SetActive(false);
            _CloseEyeObj.gameObject.SetActive(true);

            _TargetMask_Shadow.SetDrawPoint(_eyebrushObj_ins.transform.GetChild(0));

            selectImgEnable(ShadowColorList);
            selectButton.imgSelected.enabled = true;
            DotweenMgr.DoPopupOpen(0f, 1f, .3f, selectButton.imgSelected.transform);
            _TargetMask_Shadow.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, shadowDrawSize, Color.clear, _EyeShadow[_idx].texture, false, true, false, false);
            _TargetMask_Shadow.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
            _TargetMask_Shadow.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
            _TargetMask_Shadow.drawEnabled = true;

            _eyebrushObj_ins.GetComponentInChildren<ParticleSystem>().Play();

            _MakingCanvas.ShowSideView(false);
        }
        else
        {
            SoundMgr.GetInst().OnClickSoundBtn();

            _TargetMask_Shadow.SetDrawingTexture((Texture2D)_MakingCanvas._ClearTex);
            _TargetMask_Shadow.gameObject.GetComponent<RawImage>().texture = _TargetMask_Shadow.tex;
        }
    }

    private bool _isPlayEff = false;
    public void SetPlayEff(bool _check) => _isPlayEff = _check;

    void OnDrag_EyeBrush(PointerEventData data, ColorButton selectButton)
    {
        //    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));

        //    if (mouseWorldPosition.x > (_test.GetComponent<RectTransform>().position).x)
        //    _test.OnDrag(data);
        //    if (_eyebrushObj_ins == null) Debug.LogError("EyebrushInst is NULL");

        //    _eyebrushObj_ins.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, data.position, data.pressEventCamera))
        {
            _test.OnDrag(data);
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        _eyebrushObj_ins.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
    }
    void OnPointUp_EyeBrush(PointerEventData data, ColorButton selectButton)
    {
        _test.OnEndDrag(data);
        selectImgEnable(ShadowColorList);
        _EyeObj.gameObject.SetActive(true);
        _CloseEyeObj.gameObject.SetActive(false);
        _TargetMask_Shadow.drawEnabled = false;
        _TargetMask_Lipstick.drawEnabled = false;
        var tempPart = _eyebrushObj_ins.GetComponentInChildren<ParticleSystem>().main;
        tempPart.stopAction = ParticleSystemStopAction.Destroy;
        _eyebrushObj_ins.GetComponentInChildren<ParticleSystem>().Stop(false);
        _eyebrushObj_ins.GetComponentInChildren<ParticleSystem>().transform.SetParent(GetComponentInParent<MakingCanvas>().transform);
        Destroy(_eyebrushObj_ins);

        _MakingCanvas.ShowSideView(true);
    }

   // Vector3 _tempTr;
    ParticleSystem _particle_lip;
    void OnPointDown_LipStick(PointerEventData data, int _idx, ColorButton selectButton)
    {
        _test.OnBeginDrag(data);
        SoundMgr.GetInst().OnClickSoundBtn();
        if (_MakingCanvas == null) Debug.LogError("Canvas NULL");
        if (_idx > 0)
        {
            if (_lipstickObj_ins == null)
                _lipstickObj_ins = Instantiate(_LipStickPref, instanceTransform);
            else // 파티클때메 그냥 디스트로이 시킴 
            {
                _lipstickObj_ins.SetActive(true);
            }

            _particle_lip = _lipstickObj_ins.GetComponentInChildren<ParticleSystem>();
            //_lipstickObj_ins.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000f);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            _lipstickObj_ins.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
            _TargetMask_Lipstick.SetDrawPoint(_lipstickObj_ins.transform.GetChild(0));

            selectImgEnable(LipstickColorList);

            selectButton.imgSelected.enabled = true;
            DotweenMgr.DoPopupOpen(0f, 1f, .3f, selectButton.imgSelected.transform);

            _TargetMask_Lipstick.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, lipstickDrawSize, Color.clear, _LipStick[_idx].texture, false, true, false, false);
            //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
            _TargetMask_Lipstick.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
            _TargetMask_Lipstick.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
            _TargetMask_Lipstick.drawEnabled = true;
            _lipstickObj_ins.GetComponentInChildren<ParticleSystem>().Play();

            _MakingCanvas.ShowSideView(false);
        }
        else
        {
            _TargetMask_Lipstick.SetDrawingTexture(_MakingCanvas._ClearTex);
            _TargetMask_Lipstick.gameObject.GetComponent<RawImage>().texture = _TargetMask_Lipstick.tex;
        }
    }
    void OnDrag_LipStick(PointerEventData data)
    {
        // Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        // if (mouseWorldPosition.x > (_test.GetComponent<RectTransform>().position).x)
        //     _test.OnDrag(data);
        // if (_lipstickObj_ins == null) Debug.LogError("EyebrushInst is NULL");
        //_lipstickObj_ins.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, data.position, data.pressEventCamera))
        {
            _test.OnDrag(data);
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        _lipstickObj_ins.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
    }
    void OnPointUp_LipStick(PointerEventData data)
    {
        _test.OnEndDrag(data);
        selectImgEnable(LipstickColorList);
        _TargetMask_Lipstick.drawEnabled = false;
        var tempPart = _lipstickObj_ins.GetComponentInChildren<ParticleSystem>().main;        
        tempPart.stopAction = ParticleSystemStopAction.Destroy;
        _lipstickObj_ins.GetComponentInChildren<ParticleSystem>().Stop(false);
        _lipstickObj_ins.GetComponentInChildren<ParticleSystem>().transform.SetParent(GetComponentInParent<MakingCanvas>().transform);        
        Destroy(_lipstickObj_ins);

        _MakingCanvas.ShowSideView(true);
    }
    /// <summary>
    /// 치크 컬러 선택시 이벤트 트리거
    /// 1. 포인터 엔터 : 패턴 마스크 변경 / 페인트툴 세팅 // 2. 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="_idx"></param>
    ///


    void OnPointDown_Cheek(PointerEventData data, int _idx, ColorButton selectButton)
    {
        _test.OnBeginDrag(data);
        SoundMgr.GetInst().OnClickSoundBtn();
        if (_MakingCanvas == null) Debug.LogError("Canvas NULL");
        if (_idx > 0)
        {
            if (_CheekBrush == null)
                _CheekBrush = Instantiate(_CheekBrushPref, instanceTransform);
            _CheekBrush.SetActive(true);
            //_CheekBrush.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000f);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            _CheekBrush.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

            selectImgEnable(CheekColorList);
            //foreach (ColorButton obj in CheekColorList)
            //{
            //    if (obj.BGImg.enabled)
            //        obj.BGImg.enabled = false;
            //}
            selectButton.imgSelected.enabled = true;
            DotweenMgr.DoPopupOpen(0f, 1f, .3f, selectButton.imgSelected.transform);


            _TargetMask_Cheek.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, cheekDrawSize, Color.clear, _Cheek[_idx].texture, false, true, false, false);
            _TargetMask_Cheek.SetDrawPoint(_CheekBrush.transform.GetChild(0));
            //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
            _TargetMask_Cheek.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
            _TargetMask_Cheek.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
            _TargetMask_Cheek.drawEnabled = true;

            _MakingCanvas.ShowSideView(false);
        }
        else
        {

            _TargetMask_Cheek.SetDrawingTexture(_MakingCanvas._ClearTex);

            _TargetMask_Cheek.gameObject.GetComponent<RawImage>().texture = _TargetMask_Cheek.tex;
        }
    }

    void OnDrag_Cheek(PointerEventData data)
    {
        //  Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        //  if (mouseWorldPosition.x > (_test.GetComponent<RectTransform>().position).x)
        //      _test.OnDrag(data);
        //  if (_CheekBrush == null) Debug.LogError("EyebrushInst is NULL");
        //_CheekBrush.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, data.position, data.pressEventCamera))
        {
            _test.OnDrag(data);
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        _CheekBrush.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
    }

    void OnPointUp_Cheek(PointerEventData data)
    {
        _test.OnEndDrag(data);
        selectImgEnable(CheekColorList);
        _TargetMask_Cheek.drawEnabled = false;
        _CheekBrush.SetActive(false);

        _MakingCanvas.ShowSideView(true);
    }

    void OnPointDown_EyeBrow(PointerEventData data, int _idx, ColorButton selectButton)
    {
        _test.OnBeginDrag(data);
        SoundMgr.GetInst().OnClickSoundBtn();
        if (_MakingCanvas == null) Debug.LogError("Canvas NULL");
        if (_idx > 0)
        {
            if (_EyeBrowBrush == null)
                _EyeBrowBrush = Instantiate(_EyeBrowBrushPref, instanceTransform);
            _EyeBrowBrush.SetActive(true);

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            _EyeBrowBrush.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

            //_EyeBrowBrush.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000f);

            // 포지션 추가
            _TargetMask_EyeBrow.SetDrawPoint(_EyeBrowBrush.transform.GetChild(0));

            selectImgEnable(EyebrowColorList);
            selectButton.imgSelected.enabled = true;
            DotweenMgr.DoPopupOpen(0f, 1f, .3f, selectButton.imgSelected.transform);

            _TargetMask_EyeBrow.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, eyeBrowDrawSize, Color.clear, _Eyebrow[_idx].texture, false, true, false, false);
            //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
            _TargetMask_EyeBrow.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
            _TargetMask_EyeBrow.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
            _TargetMask_EyeBrow.drawEnabled = true;

            _EyeBrowBrush.GetComponentInChildren<ParticleSystem>().Play();

            _MakingCanvas.ShowSideView(false);
        }
        else
        {
            _TargetMask_EyeBrow.SetDrawingTexture(_MakingCanvas._ClearTex);

            _TargetMask_EyeBrow.gameObject.GetComponent<RawImage>().texture = _TargetMask_EyeBrow.tex;
        }

    }
    void OnDrag_EyeBrow(PointerEventData data)
    {
        //if (mouseWorldPosition.x > (_test.GetComponent<RectTransform>().position).x)
        //    _test.OnDrag(data);
        //if (_EyeBrowBrush == null) Debug.LogError("EyebrushInst is NULL");

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, data.position, data.pressEventCamera))
        {
            _test.OnDrag(data);
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        _EyeBrowBrush.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
    }
    void OnPointUp_EyeBrow(PointerEventData data)
    {
        _test.OnEndDrag(data);
        selectImgEnable(EyebrowColorList);
        _TargetMask_EyeBrow.drawEnabled = false;
        var tempPart = _EyeBrowBrush.GetComponentInChildren<ParticleSystem>().main;
        tempPart.stopAction = ParticleSystemStopAction.Destroy;
        _EyeBrowBrush.GetComponentInChildren<ParticleSystem>().Stop(false);
        _EyeBrowBrush.GetComponentInChildren<ParticleSystem>().transform.SetParent(GetComponentInParent<MakingCanvas>().transform);
        Destroy(_EyeBrowBrush);
        //_EyeBrowBrush.GetComponentInChildren<ParticleSystem>().Stop();

        _MakingCanvas.ShowSideView(true);
    }

    public void DesClickLensTool()
    {
        _LensToolSelect = false;

        for (int i = 0; i < _test.content.childCount; i++)
        {
            _test.content.GetChild(i).gameObject.SetActive(false);
        }
    }
  
}
