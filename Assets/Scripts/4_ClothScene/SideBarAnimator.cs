using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SideBarAnimator : MonoBehaviour
{
    public Button btnOpen;
    public Button btnClose;

    private RectTransform rt;

    private void Awake()
    {
        btnOpen.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        rt = gameObject.GetComponent<RectTransform>();
    }

    public void Open()
    {
        rt.DOAnchorPosX(0, 0.2f)
            .SetEase(Ease.Linear);

        btnClose.gameObject.SetActive(true);
        btnOpen.gameObject.SetActive(false);
    }

    public void Close()
    {
        rt.DOAnchorPosX(302.0f, 0.2f)
            .SetEase(Ease.Linear);

        btnClose.gameObject.SetActive(false);
        btnOpen.gameObject.SetActive(true);
    }

    /*
    MenuCreateor _AnimMC;
    public Animator _Anim;
    MakingStairBtn _MakingBt;
   public OHBtnChecker _ohBtn;
    private SlidePannel _slider;
    public Sprite[] _OHBtnSp;

    void Start()
    {
        _AnimMC = FindObjectOfType<MenuCreateor>();
        _Anim = GetComponent<Animator>();
        _slider = GetComponentInParent<SlidePannel>();
        if (_MakingBt == null) _MakingBt = FindObjectOfType<MakingStairBtn>();
        _MakingBt.SetSideBar(this);
        _MakingBt.SetOHBtn(_ohBtn);
    }

    public void AnimEv_HideCreate()
    {
        _AnimMC.AnimationEv_HideCreate();
    }

    public void AnimEv_SelectBg()
    {
        _MakingBt.FirstSetting_BG();
    }
    private void OnEnable()
    {
        _ohBtn.GetComponent<Image>().sprite = _OHBtnSp[1];
        _ohBtn.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

    }
    public void AnimEv_OH()
    {
        _ohBtn.GetComponent<Image>().sprite = _OHBtnSp[0];
        _ohBtn.GetComponent<RectTransform>().SetParent(_slider.GetComponent<RectTransform>());
        DotweenMgr.DoLocalMoveX(-50f, .3f, _ohBtn.GetComponent<RectTransform>());
        gameObject.SetActive(false);
    }
    public void AnimEV_Hide()
    {
        gameObject.SetActive(false);
    }
    */
}
