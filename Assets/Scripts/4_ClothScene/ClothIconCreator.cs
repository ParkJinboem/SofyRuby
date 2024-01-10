using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 아이콘 생성 및 착용
/// </summary>
public class ClothIconCreator : MonoBehaviour
{
    public SideBarAnimator sideBarAnimator;
    public PoolSystem ps;
    public ScrollRect scrollRect;
    public Transform trClothMenus;
    public List<ClothMenu> clothMenus;
    public Transform trDressHolder;
    public List<DressHolder> dressHolders;
    public Image _HairTarget;
    public Image _lockIconPref;

    private IntroCanvas _WorldCanvas;

    private void Awake()
    {
        ClothEvent.OnClickMenu += HandlerClickMenu;
        ClothEvent.OnWearCloth += HandlerWearCloth;
        ClothEvent.OnClothLoaded += HandlerClothLoaded;
    }

    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        ClothEvent.OnClickMenu -= HandlerClickMenu;
        ClothEvent.OnWearCloth -= HandlerWearCloth;
        ClothEvent.OnClothLoaded -= HandlerClothLoaded;
    }

    public void DestroyedScene(Action callback)
    {
        callback();
    }

    /// <summary>
    /// 부하가 심하게 걸려서 나눠둠
    /// 리소스들 로드해서 들고있음
    /// </summary>
    public void Init()
    {
        Resources.UnloadUnusedAssets();
        _WorldCanvas = FindObjectOfType<IntroCanvas>();

        clothMenus = new List<ClothMenu>(trClothMenus.GetComponentsInChildren<ClothMenu>());
        dressHolders = new List<DressHolder>(trDressHolder.GetComponentsInChildren<DressHolder>());

        Invoke("Init2", .3f);
    }

    void Init2()
    {
        for (int i = 0; i < clothMenus.Count; i++)
        {
            ClothMenu clothMenu = clothMenus[i];
            DressHolder dressHolder = dressHolders.Find(x => x.ms == clothMenu.ms);
            dressHolder.SetIdx(clothMenu.DefaultIdx);
            if (clothMenu.IsDefaultWear)
            {
                dressHolder.Show();
            }
            else
            {
                dressHolder.Hide();
            }
        }
        CheckAllHide();

        Invoke("SetOffLoadPannel", .3f);
    }

    private void SetOffLoadPannel()
    {
        WorldMgr.GetInst().OnOffLoadPannel(false);
        if (SaveLoadMgr.GetInst().CheckSceneLoad())
        {
            SaveLoadMgr.GetInst().SetCheckSceneLoad(false);
            SaveLoadMgr.GetInst().LoadData();
        }

        // 로딩 완료 후 배너 광고
        AdsManager.Instance.RequestBanner();
        AdsManager.Instance.WatchAdWithFuncUse();
    }

    private void OnClick_ADV_Ok()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        _WorldCanvas.OnClickADVPannel(false);
    }

    bool DisLock = false;
    /// <summary>
    /// 잠금 아이템 눌렀을시 
    /// </summary>
    public void OnClick_ADV_No()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        _WorldCanvas.OnClickADVPannel(false);
    }

    /// <summary>
    /// 광고를 볼것인지 물어보는 팝업에서 Yes로 들어올 경우
    /// </summary>
    /// <returns></returns>
    public bool OnClick_ADV_OK()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        _WorldCanvas.OnClickADVPannel(false);
        DisLock = true;
        return DisLock;
    }

    private void HandlerClickMenu(ClothMenu clothMenu)
    {
        SoundMgr.GetInst().OnClickSoundBtn();

        CreateIcon(clothMenu);
        sideBarAnimator.Open();
    }

    private void CreateIcon(ClothMenu clothMenu)
    {
        scrollRect.verticalNormalizedPosition = 1;

        DressHolder checkDressHolder = dressHolders.Find(x => x.ms == clothMenu.ms);

        ps.Clear();
        for (int i = 0; i < clothMenu.spClothIcons.Length; i++)
        {
            Transform trIcon = ps.GetObjectFromPool(clothMenu.trClothIcon.name, true);
            ColorButton colorButton = trIcon.GetComponent<ColorButton>();
            colorButton.SetClothMenu(clothMenu, i + 1, checkDressHolder.IsShow && checkDressHolder.Idx == i + 1);
            trIcon.gameObject.SetActive(true);
        }
    }

    private void HandlerWearCloth(ClothMenu clothMenu, int idx)
    {
        SoundMgr.GetInst().OnPlayOneShot("se_01");

        DressHolder checkDressHolder;
        if (idx == 1 &&
            clothMenu.useRemove)
        {
            // 해제
            checkDressHolder = dressHolders.Find(x => x.ms == clothMenu.ms);
            checkDressHolder.SetIdx(idx);
            checkDressHolder.Hide();
        }
        else
        {
            // 착용
            for (int i = 0; i < clothMenu.disableClothes.Count; i++)
            {
                checkDressHolder = dressHolders.Find(x => x.ms == clothMenu.disableClothes[i]);
                if (checkDressHolder != null)
                {
                    checkDressHolder.Hide();
                }
            }

            DressHolder dressHolder = dressHolders.Find(x => x.ms == clothMenu.ms);
            dressHolder.SetIdx(idx);
            dressHolder.Show();

            if (dressHolder.Effect != null)
            {
                //EffectManager.Instance.Show(dressHolder.Effect.EffectName, dressHolder.Effect.Position);
                EffectManager.Instance.Show(dressHolder.Effect.EffectName, dressHolder.Effect.Position, dressHolder.Effect.effScale);
            }
        }

        CheckAllHide();
    }

    private void HandlerClothLoaded()
    {
        CheckAllHide();
    }

    private void CheckAllHide()
    {
        // 상의 체크
        CheckAllHide(ClothMenu.CheckPosition.Top, MenuSelector.Shirts);
        // 하의 체크
        CheckAllHide(ClothMenu.CheckPosition.Bottom, MenuSelector.Pants);
        // 헤어 체크
        CheckAllHide(ClothMenu.CheckPosition.None, MenuSelector.Hair);
    }

    private void CheckAllHide(ClothMenu.CheckPosition checkPosition, MenuSelector initMS)
    {
        bool isAllHide = true;
        if (checkPosition == ClothMenu.CheckPosition.None)
        {
            for (int i = 0; i < clothMenus.Count; i++)
            {
                if (clothMenus[i].disableClothes.Contains(initMS))
                {
                    DressHolder dressHolder = dressHolders.Find(x => x.ms == clothMenus[i].ms);
                    if (dressHolder.IsShow)
                    {
                        isAllHide = false;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < clothMenus.Count; i++)
            {
                if (clothMenus[i].checkPosition == ClothMenu.CheckPosition.All ||
                    clothMenus[i].checkPosition == checkPosition)
                {
                    DressHolder dressHolder = dressHolders.Find(x => x.ms == clothMenus[i].ms);
                    if (dressHolder.IsShow)
                    {
                        isAllHide = false;
                        break;
                    }
                }
            }
        }

        if (isAllHide)
        {
            DressHolder dressHolder = dressHolders.Find(x => x.ms == initMS);
            dressHolder.Show(isAllHide);
        }
    }
}
