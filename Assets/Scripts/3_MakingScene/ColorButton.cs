using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public Image imgIcon;
    public Image imgSelected;
    public Image imgLock;

    private Button btn;
    private ClothMenu clothMenu;
    private int idx;
    private bool isSelected;

    private void Awake()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        ClothEvent.OnWearCloth += HandlerWearCloth;
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDestroy()
    {
        ClothEvent.OnWearCloth -= HandlerWearCloth;
    }

    private void Init()
    {
        if (clothMenu != null)
        {
            bool isHave = WorldMgr.GetInst().GetHaveCloth(clothMenu.ms, idx);
            imgIcon.sprite = clothMenu.spClothIcons[idx - 1];
            imgSelected.enabled = isSelected;
            imgLock.enabled = !isHave;
        }
    }

    public void SetClothMenu(ClothMenu clothMenu, int idx, bool isSelected)
    {
        this.clothMenu = clothMenu;
        this.idx = idx;
        this.isSelected = isSelected;
    }

    public void OnClick()
    {
        SoundMgr.GetInst().OnClickSoundBtn();
        bool isHave = WorldMgr.GetInst().GetHaveCloth(clothMenu.ms, idx);
        if (isHave)
        {
            ClothEvent.WearCloth(clothMenu, idx);
        }
        else
        {
            ShowAd();
        }
    }

    private void HandlerWearCloth(ClothMenu clothMenu, int idx)
    {
        if (this.clothMenu.ms == clothMenu.ms)
        {
            isSelected = idx == this.idx;
            imgSelected.enabled = isSelected;
        }
    }

    public void ShowAd()
    {
        WorldMgr.GetInst().ShowAlert(new AlertViewOptions()
        {
            message = LocalizeManager.Instance.Localization("AdvPop"),
            okButtonDelegate = () =>
            {
                AdsManager.Instance.WatchVideoWithClothItem(acs =>
                {
                    switch (acs)
                    {
                        case AdsManager.AdsCallbackState.Success:
                            WorldMgr.GetInst().SetHaveCloth(clothMenu.ms, idx);
                            Init();
                            break;
                        case AdsManager.AdsCallbackState.Loading:
                            WorldMgr.GetInst().ShowMessageAlert(LocalizeManager.Instance.Localization("LoadAd"));
                            break;
                    }
                });
            }
        });
    }
}
