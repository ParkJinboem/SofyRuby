using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CATEGORY
{
    LENS = 0,
    SHADOW,
    LIPSTICK,
    CHEEK,
    EYEBROW
}

public class SideViewObj : MonoBehaviour
{
    LensTool _childTool_Lens;
    bool isAlready = false;
    CATEGORY _cate;
    public GameObject _CreamMaskSet;
    bool _isActiveSet = false;
    private Vector2 ohBtnPos;

    public SwapToolBoard _ParentTarget;

    public Sprite[] _OHSprte;

    private void Start()
    {
        _childTool_Lens = GetComponentInChildren<LensTool>();
    }
    private void OnEnable()
    {
        _OHBtn.GetComponent<Image>().sprite = _OHSprte[1];
        
        _OHBtn.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
        ohBtnPos = _OHBtn.GetComponent<RectTransform>().anchoredPosition;
        ohBtnPos.x = -230f;
        _OHBtn.GetComponent<RectTransform>().anchoredPosition = ohBtnPos;

        //DotweenMgr.DoLocalMoveX(-247f, .3f, _OHBtn.GetComponent<RectTransform>());

    }
    public LensTool _getLensTool() { return _childTool_Lens; }

   public OHBtnChecker _OHBtn;

    public void OnClickSideViewObj(int _Category)
    {
        if ((CATEGORY)_Category == _cate && GetComponent<Animator>().GetBool("isShow")) {
            if ((CATEGORY)_Category == CATEGORY.EYEBROW) _childTool_Lens._isPickUpBrush = true;
            return;
        }

        _cate = (CATEGORY)_Category;

        SoundMgr.GetInst().OnClickSoundBtn();

        if (!gameObject.activeInHierarchy) _isActiveSet = true;
        gameObject.SetActive(true);
        _CreamMaskSet.SetActive(false);
        if (isAlready)
        {
            if (_isActiveSet)
            {
                _isActiveSet = false;
                if (!GetComponent<Animator>().GetBool("isShow"))
                {
                    DesReLoad();
                    GetComponent<Animator>().SetBool("isShow", true);
                    return;
                }
                else { GetComponent<Animator>().SetTrigger("AlreadyIns"); return; }
            }
            GetComponent<Animator>().SetTrigger("AlreadyIns");
            return;
        }
        DesClickSideViewObj();
        GetComponent<Animator>().SetBool("isShow", true);
       
        switch (_cate)
        {
            case CATEGORY.LENS:
                _childTool_Lens.OnClickLensTool(CATEGORY.LENS);
                break;
            case CATEGORY.SHADOW:
                _childTool_Lens.OnClickLensTool(CATEGORY.SHADOW);
                break;
            case CATEGORY.LIPSTICK:
                _childTool_Lens.OnClickLensTool(CATEGORY.LIPSTICK);
                break;
            case CATEGORY.CHEEK:
                _childTool_Lens.OnClickLensTool(CATEGORY.CHEEK);
                break;                
            case CATEGORY.EYEBROW:
                _childTool_Lens.OnClickLensTool(CATEGORY.EYEBROW);
                break;
        }
        isAlready = true;
    }
    public void DesClickSideViewObj()
    {
        //if (_childTool_Lens._CheekBrush.activeInHierarchy) _childTool_Lens._CheekBrush.SetActive(false);

        GetComponent<Animator>().SetBool("isShow", false);
        _childTool_Lens.DesClickLensTool();
        isAlready = false;
    }
    

    void DesReLoad() // 애니메이션 트리거 콜 펑션
    {
        _childTool_Lens.DesClickLensTool();
            //_childTool_Lens._CheekBrush.SetActive(false);
        switch (_cate)
        {
            case CATEGORY.LENS:
                _childTool_Lens.OnClickLensTool(CATEGORY.LENS);
                break;
            case CATEGORY.SHADOW:
                _childTool_Lens.OnClickLensTool(CATEGORY.SHADOW);
                break;
            case CATEGORY.LIPSTICK:
                _childTool_Lens.OnClickLensTool(CATEGORY.LIPSTICK);
                break;
            case CATEGORY.CHEEK:
                //if (!_childTool_Lens._CheekBrush.activeInHierarchy)
                //    _childTool_Lens._CheekBrush.SetActive(true);
                _childTool_Lens.OnClickLensTool(CATEGORY.CHEEK);
                break;
            case CATEGORY.EYEBROW:
                _childTool_Lens.OnClickLensTool(CATEGORY.EYEBROW);
                break;
        }
        //isAlready = true;
    }


    public void ActiveSideBar(int _test) // 애니메이션 트리거 콜
    {
        if (_test == 1)
        {
            DesReLoad();
        }
        else gameObject.SetActive(true);
    }
    
    public void ResetActive()
    {
        _OHBtn.GetComponent<Image>().sprite = _OHSprte[0];
        _OHBtn.GetComponent<RectTransform>().SetParent(_ParentTarget.GetComponent<RectTransform>());
        ohBtnPos = _OHBtn.GetComponent<RectTransform>().anchoredPosition;
        ohBtnPos.x = -70f;
        _OHBtn.GetComponent<RectTransform>().anchoredPosition = ohBtnPos;
        //DotweenMgr.DoLocalMoveX(-50f, .3f, _OHBtn.GetComponent<RectTransform>());
        gameObject.SetActive(false);
    }

    public void OutOHBtn()
    {

    }
}
