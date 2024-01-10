using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화장씬에서 잠금 기능 들어가는것이 머리밖에 없어서 별도로 구현해둠, 구현부는 ClothScene에서 옷 잠금 과 비슷하게 작동
/// </summary>
public class HairColorScrollView : MonoBehaviour
{
    [SerializeField] GameObject _ColorSelectObj;
    [SerializeField] GameObject _TargetHair;
    [SerializeField] ScrollRect _scroll;
    [SerializeField] Sprite[] _Hair;
    [SerializeField] Sprite[] _HairColor;

    private void Start()
    {
        _scroll = GetComponent<ScrollRect>();
        _Hair = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.HAIR);
        _HairColor = MakingMgr.GetInst().SpriteLoader(SpriteSelectorIDX.HAIRCOLOR);

        for (int i = 0; i < _HairColor.Length; i++)
        {
            int idx = i;

            GameObject _instObj = Instantiate(_ColorSelectObj, _scroll.content);
            _instObj.transform.GetChild(0).GetComponent<Image>().sprite = _HairColor[idx];
            Image imgLock = _instObj.transform.GetChild(1).GetComponent<Image>();
            Button btnWear = _instObj.AddComponent<Button>();
            btnWear.onClick.AddListener(() => OnClick(imgLock, idx));

            bool isHave = WorldMgr.GetInst().GetHaveCloth(MenuSelector.Hair, idx);
            imgLock.enabled = !isHave;
        }
    }

    public void OnClick(Image imgLock, int idx)
    {
        SoundMgr.GetInst().OnPlayOneShot("se_01");
        bool isHave = WorldMgr.GetInst().GetHaveCloth(MenuSelector.Hair, idx);
        if (isHave)
        {
            _TargetHair.GetComponent<Image>().sprite = _Hair[idx];
            SaveLoadMgr.GetInst().SettingInfo_hair(idx);
        }
        else
        {
            ShowAd(imgLock, idx);
        }
    }

    public void ShowAd(Image imgLock, int idx)
    {
        WorldMgr.GetInst().ShowAlert(new AlertViewOptions()
        {
            message = LocalizeManager.Instance.Localization("AdvPop"),
            okButtonDelegate = () =>
            {
                AdsManager.Instance.WatchVideoWithMakingItem(acs =>
                {
                    switch (acs)
                    {
                        case AdsManager.AdsCallbackState.Success:
                            WorldMgr.GetInst().SetHaveCloth(MenuSelector.Hair, idx);
                            imgLock.enabled = false;
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
