using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 메인씬 + 전체 단계 에서의 기능 구현
/// </summary>
public class MakingStairBtn : MonoBehaviour
{
    [SerializeField]
    private Image _BGImg;
    public SwapToolBoard _board;
    private SideBarAnimator _sideBar;
    private DownToolBar_Checker _downBar;
    private bool _isGoNextBG = false;
    private bool _isPrevCheck = false;
    private bool _isCheckPrevIns = false;
    [SerializeField]
    private Sprite[] _BG_sp;
    private Sprite[] _BG_IconSP;
    private int _BG_idx = 0;

    public GameObject _BGPref_Btn;
    public RectTransform _CompleteBtn;

    public Image[] _myBtn;
    public Image[] GetMyBtn() { return _myBtn; }
    public Image[] StairBtn
    {
        get { return _myBtn; }
    }

    private OHBtnChecker _ohb;

    public void SetCompletePref(GameObject _obj)=>
        _CompleteBtn = _obj.GetComponent<RectTransform>() ;

    public void SetSideBar(SideBarAnimator _obj) => _sideBar = _obj;
    public void SetOHBtn(OHBtnChecker _obj) => _ohb = _obj;

    public void Next_Btn()
    {

        WorldMgr.GetInst().OnOffOptionPannel(false);
        if (!WorldMgr.GetInst().GetPrevSceneChecker())
            SoundMgr.GetInst().OnClickSoundBtn();

        if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
        {
            _CompleteBtn.gameObject.SetActive(true);
            _CompleteBtn.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            _myBtn[1].gameObject.SetActive(false);
            //DotweenMgr.DoLocalMoveX(-90f, .3f, _myBtn[0].GetComponent<RectTransform>());
        }
        else
        {
            //DotweenMgr.DoLocalMoveX(-250f, .3f, _myBtn[0].GetComponent<RectTransform>());
            _myBtn[1].gameObject.SetActive(true);
        }
        if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName && !_isGoNextBG)
        {

            if(_sideBar == null)
                _sideBar = FindObjectOfType<SideBarAnimator>();
            if (_downBar == null)
            {
                _BGImg = FindObjectOfType<ClothBG_Checker>().GetComponent<Image>();
                _downBar = FindObjectOfType<DownToolBar_Checker>();
            }
          
            //_BG_sp = MakingMgr.GetInst().SpriteLoader(MENUSELECTOR.CLOTHBACKGROUND);
            //_BG_IconSP = MakingMgr.GetInst().SpriteLoader(MENUSELECTOR.CLOTHBACKGROUNDICON);

            _isGoNextBG = true;
           
            
        } if (_isPrevCheck) return;
        if (_isGoNextBG) // 배경선택으로 넘어가는 곳
        {
            _ohb.gameObject.SetActive(false);
            _sideBar.gameObject.SetActive(true);
            _sideBar.GetComponentInChildren<ClothIconCreator>().gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            //DotweenMgr.DoLocalMoveX(247f, .3f, _sideBar.GetComponent<RectTransform>());
            // 1208 임시주석
            //if (_sideBar._Anim.GetBool("isSelect")) // 사이드바 켜져있을때
            //    _sideBar._Anim.SetTrigger("BGSelect");
            //else _sideBar.gameObject.SetActive(false);


            _downBar.GetComponent<RectTransform>().DOAnchorPosY(-300f, .5f);
            
            //DotweenMgr.DoSizeImage(.7f, 1f, .5f, _CompleteBtn.gameObject);
            //_downBar.gameObject.SetActive(false);
            //RectTransform _rect = FindObjectOfType<IntroCanvas>().GetComponent<RectTransform>();

            //DotweenMgr.DoLocalMoveY(_rect.sizeDelta.y, .5f, this.transform);
            _isPrevCheck = true;
            return;
        }

        if (_board == null) _board = FindObjectOfType<SwapToolBoard>();
        _board.SwapNext();
    }
    public void FirstSetting_BG()
    {
        ScrollRect _temprt = _sideBar.GetComponentInChildren<ScrollRect>();
        for (int i = 0; i < _temprt.content.childCount; i++)
            _temprt.content.GetChild(i).gameObject.SetActive(false);


        if (!_isCheckPrevIns)
        {
            for (int i = 0; i < _BG_sp.Length; i++)
            {
                int tempidx = i;
                GameObject _obj = Instantiate(_BGPref_Btn, _temprt.content);
                _obj.GetComponent<Image>().sprite = _BG_IconSP[i];
                _obj.AddComponent<Button>();
                _obj.GetComponent<Button>().onClick.AddListener(() => BGBtnOnclick(tempidx));
                _obj.tag = "BackGround";
            }
            _isCheckPrevIns = true;
        }
        else
        {
            for(int i = 0; i<_temprt.content.childCount; i++)
            {
                if(_temprt.content.GetChild(i).CompareTag("BackGround"))
                {
                    _temprt.content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        
    }
    public void BGBtnOnclick(int _idx)
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        _BGImg.sprite = _BG_sp[_idx];
        SaveLoadMgr.GetInst().SettingInfo_BG(_idx);
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene(GameManager.MainSceneName);
        _myBtn[1].gameObject.SetActive(true);
    }

    public void Prev_Btn()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        WorldMgr.GetInst().OnOffOptionPannel(false);

        if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
        {
            WorldMgr.GetInst().ShowAlert(new AlertViewOptions()
            {
                message = LocalizeManager.Instance.Localization("MoveMakeup"),
                okButtonDelegate = () =>
                {
                    Prev();
                }
            });
        }
        else
        {
            Prev();
        }
    }

    private void Prev()
    {
        _myBtn[1].gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
        {
            _isGoNextBG = false;
            if (!_isPrevCheck)
            {
                _isPrevCheck = false;
                _isCheckPrevIns = false;
                WorldMgr.GetInst().OnOffLoadPannel(true);
                WorldMgr.GetInst().SetPrevSceneChecker(true);
                Invoke("LoadMainScene", 1f);
                return;
            }
            try
            {
                _downBar.GetComponent<RectTransform>().DOAnchorPosY(89f, .5f);
            }
            catch (Exception e)
            {
                Debug.Log("NO");
            }
            _CompleteBtn.gameObject.SetActive(false);
            _isPrevCheck = false;
            return;
        }
        if (_board == null) _board = FindObjectOfType<SwapToolBoard>();
        _board.SwapPrev();

    }
}
