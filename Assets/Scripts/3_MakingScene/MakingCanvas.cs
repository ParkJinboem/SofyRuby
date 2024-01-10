using System;
using UnityEngine;
using UnityEngine.UI;

public enum TOOLS
{
    SOAP = 0,
    PIMPLE,
    SHOWER,
    PACK = 3,
    CUCUMBLE,
    CREAM = 5,
    LENS,
    EYESHADOW = 7,
    LIPSTICK,
    CHIK = 9,
    EYEBROW,
    EYEBRUSH,
    HAIR,
    TOWEL,
    NOSEPACK = 14
}

/// <summary>
/// 사이드바가 나올 필요가 없는 단순 바르는 툴들의 기능 구현
/// </summary>
public class MakingCanvas : MonoBehaviour
{
    Vector3 _resetPos;
    
    Vector2 _resetSize;

    Sprite[] _tools;
    Sprite _origin;
    Sprite _eyeorigin;
    //public Sprite _CloseEye;
    public GameObject _jujuEyes;
    public GameObject _closeEye;
    [SerializeField]
    private GameObject[] _cucumbles;

    [SerializeField]
    private SideViewObj _sideBar;

    public GameObject _MaskTarget;
    public GameObject _CreamTarget;
    public Texture2D _BubbleMask;
    public Texture2D _GreenMask;
    public Texture2D _CreamMask;
    public Texture2D _ClearTex;
    public Texture2D _DropMask;
    public Material _ClearMat;

    [SerializeField] int bubbleDrawSize = 32;
    [SerializeField] int showerDrawSize = 50;
    [SerializeField] int packDrawSize = 32;
    [SerializeField] int creamDrawSize = 24;
    [SerializeField] int towelDrawSize = 50;

    public GameObject _ShowerParticle;
    AdvancedMobilePaint.AdvancedMobilePaint _paint;
    AdvancedMobilePaint.AdvancedMobilePaint _creamPaint;

    private Pimples _pimples;

    #region 상태 아이콘 스프라이트
    public Image _NowStatIcon;
    
    public Sprite _Towel;
    public Sprite _Soap;
    public Sprite _Shower;
    public Sprite _Pack;
    public Sprite _Pimple;
    public Sprite _Cream;
    public Sprite _Cucumble;
    public Sprite _NosePack;
    #endregion

    TOOLS _toolselector;

    [Header("SideView")]
    [SerializeField] private CanvasGroup sideViewCanvasGroup;

    [Header("Ads")]
    [SerializeField] private RectTransform bannerCanvasRoot;
    [SerializeField] private RectTransform[] bannerChangeTransforms; // 배너 광고로 위치 변경할 오브젝트 리스트

    private void Awake()
    {
        AdsManager.OnLoadedBanner += OnLoadedBanner;
    }

    private void Start()
    {
        _paint = _MaskTarget.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
        _creamPaint = _CreamTarget.gameObject.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>();
        _sideBar = FindObjectOfType<SideViewObj>();
        _sideBar.gameObject.SetActive(false);
        GetComponent<Canvas>().worldCamera = Camera.main;
        _pimples = FindObjectOfType<Pimples>();

        MainSceneMgr.GetInst().LoadGirlHolderOnLoaded();
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

    public void SetTools(int _tool) => _toolselector = (TOOLS)_tool;
    public void OnPointerDown(RectTransform _target)
    {
        if(_tools == null)
            _tools = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.TOOL);

        //_eyeorigin =_jujuEyes.GetComponent<Image>().sprite;
        //_jujuEyes.GetComponent<Image>().sprite = _CloseEye;

        _closeEye.SetActive(true);
        _jujuEyes.SetActive(false);

        _resetPos = _target.position;
        _resetSize = _target.rect.size;
        _origin = _target.gameObject.GetComponent<Image>().sprite;
        Sprite _tool = null;

        _sideBar.DesClickSideViewObj();
        _CreamTarget.gameObject.SetActive(false);

        Color c = Color.clear;
        switch (_toolselector)
        {
            case TOOLS.SOAP:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Soap") _tool = _obj;                
                _paint.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, bubbleDrawSize, c, _BubbleMask, false, true, false, false);
              //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
                _paint.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
                _paint.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
                _paint.drawEnabled = true;

                _paint.SetDrawPoint(_target.GetChild(0));
                _paint.SetDrawPos(new Vector3());
                _MaskTarget.gameObject.GetComponent<RawImage>().texture = _paint.tex;
                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _Soap;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);

                //_paint.drawMode = AdvancedMobilePaint.DrawMode.Default;
                // _paint.drawEnabled = true;
                //Camera.main.GetComponent<SpaSalonController>().canBlink = false;
                //Camera.main.GetComponent<SpaSalonController>().blinkCounter++;
                break;
            case TOOLS.PIMPLE:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Pimple") _tool = _obj;
                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _Pimple;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);
                _target.GetComponent<PimpleTool>()._check = true;
                BoxCollider[] _tempBox = _pimples.GetComponentsInChildren<BoxCollider>();
                foreach(BoxCollider _box in _tempBox)
                {
                    _box.enabled = true;
                }
                //_target.GetComponentInChildren<ParticleSystem>().Play();

                break;

            case TOOLS.SHOWER:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Shower") _tool = _obj;
                _target.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                //_paint.SetDrawingTexture(_BubbleMask);
                _paint.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, showerDrawSize, c, _DropMask, false, true, false, false);
                //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
                _paint.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
                _paint.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
                _paint.drawEnabled = true;
                _paint.SetDrawPoint(_target.GetChild(0));
                _paint.SetDrawPos(new Vector3(-240f, 170f));
                _MaskTarget.gameObject.GetComponent<RawImage>().texture = _paint.tex;

                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _Shower;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);
                // 이전 버전(지우기)
                //Color c = Color.red;
                //c.a = 0f;
                //_paint.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, 64, c, null, false, true, false, false);
                //_paint.useLockArea = false;
                //_paint.useMaskLayerOnly = false;
                //_paint.useThreshold = false;
                //_paint.drawEnabled = true;
                //_paint.SetDrawPos(new Vector3(-250f, 200f));
                //_MaskTarget.gameObject.GetComponent<RawImage>().texture = _paint.tex;
                break;
            case TOOLS.PACK:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Massage") _tool = _obj;
                _paint.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, packDrawSize, c, _GreenMask, false, true, false, false);
                //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
                _paint.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
                _paint.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
                _paint.drawEnabled = true;

                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _Pack;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);


                _paint.SetDrawPoint(_target.GetChild(0));
                _paint.SetDrawPos(new Vector3(-130f, 120f));
                _MaskTarget.gameObject.GetComponent<RawImage>().texture = _paint.tex;
                break;
            //case TOOLS.CUCUMBLE:
            //    foreach (Sprite _obj in _tools)
            //        if (_obj.name == "Cucumble_Fix") _tool = _obj;
            //    _target.GetComponent<CucumbleTool>()._check = true;
            //    _cucumbles = _target.GetComponent<CucumbleTool>().GetCucumbles();
            //    _cucumbles[_target.GetComponent<CucumbleTool>()._cucumbleIdx].SetActive(true);
            //    //_target.GetComponent<PimpleTool>()._check = true;
            //    return;
            case TOOLS.CREAM:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Cream") _tool = _obj;
                _CreamTarget.SetActive(true);
                _creamPaint.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, creamDrawSize, c, _CreamMask, false, true, false, false);
                //  _paint.SetVectorBrush(VectorBrush.Circle, 48, Color.white, _BubbleMask, false, true, true, true);
                _creamPaint.brushMode = AdvancedMobilePaint.BrushProperties.Pattern;
                _creamPaint.drawMode = AdvancedMobilePaint.DrawMode.Pattern;
                _creamPaint.drawEnabled = true;
                _creamPaint.SetDrawPoint(_target);

                _CreamTarget.gameObject.GetComponent<RawImage>().texture = _creamPaint.tex;
                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _Cream;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);
                break;

                // 스위치문에서 빈 곳은 LensTool 스크립트에 구현 
            case TOOLS.LENS:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Lens") _tool = _obj;
                break;
            case TOOLS.EYESHADOW:
                break;
            case TOOLS.LIPSTICK:
                break;
            case TOOLS.CHIK:
                break;
            case TOOLS.EYEBROW:
                break;
            case TOOLS.EYEBRUSH:
                break;
            case TOOLS.HAIR:
                break;
            case TOOLS.TOWEL:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "Towel") _tool = _obj;
                c = Color.red;
                c.a = 0f;
                _paint.SetVectorBrush(AdvancedMobilePaint.VectorBrush.Circle, towelDrawSize, c, null, false, true, false, false);
                _paint.useLockArea = false;
                _paint.useMaskLayerOnly = false;
                _paint.useThreshold = false;
                _paint.drawEnabled = true;
                _paint.SetDrawPoint(_target.GetChild(0));
                _paint.SetDrawPos(new Vector3());
                _MaskTarget.gameObject.GetComponent<RawImage>().texture = _paint.tex;
                _target.GetComponentInChildren<ParticleSystem>().Play();

                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _Towel;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);
                break;
            case TOOLS.NOSEPACK:
                foreach (Sprite _obj in _tools)
                    if (_obj.name == "NosePack") _tool = _obj;

                _NowStatIcon.gameObject.SetActive(true);
                _NowStatIcon.sprite = _NosePack;
                DotweenMgr.DoPopupOpen(0f, 1f, .6f, _NowStatIcon.transform);
                break;
            default:
                Debug.LogError("Not Set TOOLS Select");
                break;
        }
        _target.gameObject.GetComponent<Image>().sprite = _tool;
        _target.sizeDelta = new Vector2(_tool.rect.xMax, _tool.rect.yMax);
        _target.gameObject.transform.position = (Vector2)Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
    }

    bool _checker = false;
    public void OnPointerDrag(RectTransform _target)
    {
        _target.gameObject.transform.position = (Vector2)Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
    }

    public void OnPointerUp(RectTransform _target)
    {
        
        DotweenMgr.DoPopupOpen(1f, 0f, .6f, _NowStatIcon.transform);

        _paint.drawEnabled = false;
        _creamPaint.drawEnabled = false;

        _closeEye.SetActive(false);
        _jujuEyes.SetActive(true);
        
        //_jujuEyes.GetComponent<Image>().sprite = _eyeorigin;

        switch (_toolselector) {

            case TOOLS.PIMPLE:
                _target.GetComponent<PimpleTool>()._check = false;
                BoxCollider[] _tempBox = _pimples.GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider _box in _tempBox)
                {
                    _box.enabled = false;
                }
                _target.GetComponentInChildren<ParticleSystem>().Stop();
                break;
            case TOOLS.SHOWER:
                _target.gameObject.GetComponentInChildren<ParticleSystem>().Stop();
                break;
            //case TOOLS.CUCUMBLE:
            //    _target.GetComponent<CucumbleTool>()._check = false;
            //    foreach (GameObject obj in _cucumbles) obj.GetComponent<RectTransform>().anchoredPosition.Set(0f, 0f);
            //    _cucumbles[_target.GetComponent<CucumbleTool>()._cucumbleIdx].SetActive(false);
            //    return;
            case TOOLS.CREAM:
                DotweenMgr.DoFadeImage(1f, 0f, 1f, _CreamTarget.GetComponent<RawImage>());
                Invoke("ClearAdvPaint", 1f);
                
                break;

            case TOOLS.TOWEL:

                _target.GetComponentInChildren<ParticleSystem>().Stop();
                break;
        }
        _target.position = _resetPos;
        _target.sizeDelta = new Vector2(_resetSize.x, _resetSize.y);
        _target.gameObject.GetComponent<Image>().sprite = _origin;

    }
    /// <summary>
    /// 마스크 클리어
    /// </summary>
    void ClearAdvPaint()
    {
      
        _CreamTarget.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().ClearImage();
        _CreamTarget.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        _CreamTarget.GetComponent<RawImage>().texture = _CreamTarget.GetComponent<AdvancedMobilePaint.AdvancedMobilePaint>().tex;
        
    }
    OHBtnChecker _ohbtn;
    public void OnOffBtn()
    {
        if (_ohbtn == null) _ohbtn = FindObjectOfType<OHBtnChecker>();
        if (!_sideBar.gameObject.activeInHierarchy)
        {
            //DotweenMgr.DoLocalMoveX(-250f, .3f, _ohbtn.GetComponent<RectTransform>());
            _sideBar.gameObject.SetActive(true);
            _sideBar.GetComponent<Animator>().SetBool("isShow", true);
        }
        else
        {
            //DotweenMgr.DoLocalMoveX(0f, .3f, _ohbtn.GetComponent<RectTransform>());
            _sideBar.GetComponent<Animator>().SetTrigger("NPBtn");
        }
    }

    public void ShowSideView(bool isShow)
    {
        sideViewCanvasGroup.alpha = isShow ? 1f : 0f;
        sideViewCanvasGroup.blocksRaycasts = isShow;
    }
}