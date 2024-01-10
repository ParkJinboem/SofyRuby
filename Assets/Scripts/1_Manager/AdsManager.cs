using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance = null;

    private BannerView bannerView;
    private RewardedAd makingItemAd;
    private RewardedAd clothItemAd;
    private InterstitialAd funcUseAd;
    private InterstitialAd saveAd;
    
    private bool isLoadedBanner;
    private bool isLoadingMakingItemAd;
    private bool isLoadingClothItemAd;
    private bool isLoadingFuncUseAd;
    private bool isLoadingSaveAd;

    private AdSize bannerAdSize = AdSize.Banner;

    [Header("AD UNIT ID")]
    public string androidAppOpenId;
    public string iOSAppOpenId;
    public string androidBannerId;
    public string iOSBannerId;
    public string androidMakingItemId;
    public string iOSMakingItemId;
    public string androidClothItemId;
    public string iOSClothItemId;
    public string androidFuncUseId;
    public string iOSFuncUseId;
    public string androidSaveId;
    public string iOSSaveId;

    public static event Action<bool> OnLoadedBanner = delegate { };
    private Action<AdsCallbackState> callback;

    public enum AdsCallbackState
    {
        Fail, // 광고 시청 실패
        Loading, // 광고 요청하고 로딩 중
        Success // 광고 시청 완료
    }

    private void OnApplicationPause(bool pause)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (!pause)
            {
                WatchAdWithAppOpen();
            }
        });
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Complete()
    {
        callback?.Invoke(AdsCallbackState.Success);
        callback = null;
    }

    /// <summary>
    /// 광고 초기화
    /// </summary>
    public void InitAds()
    {
        RequestAppOpen();
        RequestMakingItemVideo();
        RequestClothItemVideo();
        RequestFuncUse();
        RequestSave();
    }

    #region 앱 오픈
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private AppOpenAd appOpenAd;
    private DateTime appOpenExpireTime;
    private bool isLockAppOpen; // 앱 오픈 일시적 잠금
    private bool isOnceLockAppOpen; // 기타 광고 종료 후 OnApplicationPause 선 호출 광고 노출되는 현상 1회 막기

    private bool IsAppOpenAdAvailable
    {
        get
        {
            return !isLockAppOpen
                    && appOpenAd != null
                    && appOpenAd.CanShowAd()
                    && DateTime.Now < appOpenExpireTime;
        }
    }

    public void SetLockAppOpen(bool isLockAppOpen)
    {
        this.isLockAppOpen = isLockAppOpen;
    }

    /// <summary>
    /// 앱 오픈 광고 요청
    /// </summary>
    public void RequestAppOpen()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        string adUnitId;
        if (GameManager.Instance.GameSettings.IsAdLive)
        {
            #if UNITY_ANDROID            
                adUnitId = androidAppOpenId;
            #elif UNITY_IPHONE
                adUnitId = iOSAppOpenId;
            #else
                adUnitId = "unexpected_platform";
            #endif
        }
        else
        {
            #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/9257395921";
            #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/5575463023";
            #else
		        adUnitId = "unexpected_platform";
            #endif
        }

        AdRequest request = new AdRequest.Builder()
            .Build();

        AppOpenAd.Load(adUnitId, request, (AppOpenAd appOpenAd, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.Log($"ads Load :{error.GetMessage()}");
            }

            if (error != null ||
                appOpenAd == null)
            {
                return;
            }

            this.appOpenAd = appOpenAd;
            appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;

            appOpenAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosedAppOpen;
            appOpenAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailedAppOpen;
            appOpenAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpenedAppOpen;
            appOpenAd.OnAdPaid += HandleAdPaidAppOpen;
            appOpenAd.OnAdImpressionRecorded += HandleAdImpressionRecordedAppOpen;
            appOpenAd.OnAdClicked += HandleAdClickedAppOpen;
        });
    }

    /// <summary>
    /// 앱 오픈 광고 시청
    /// </summary>
    private void WatchAdWithAppOpen()
    {
        if (!GameManager.Instance.GameSettings.UseAd)
        {
            return;
        }

        if (IsAppOpenAdAvailable)
        {
            if (isOnceLockAppOpen)
            {
                isOnceLockAppOpen = false;
                return;
            }

            appOpenAd.Show();
        }
        else
        {
            RequestAppOpen();
        }
    }

    private void HandleAdFullScreenContentOpenedAppOpen()
    {
        ActiveBanner(false);
    }

    private void HandleAdFullScreenContentClosedAppOpen()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
        RequestAppOpen();
        ActiveBanner(true);
    }

    private void HandleAdFullScreenContentFailedAppOpen(AdError error)
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
        RequestAppOpen();
    }

    private void HandleAdPaidAppOpen(AdValue adValue) { }
    private void HandleAdImpressionRecordedAppOpen() { }
    private void HandleAdClickedAppOpen() { }
    #endregion 앱 오픈

    #region 배너
    public float GetBannerHeight(Rect canvasSize)
    {
        //샘플데이터에 맞게 간격 수정_240102 박진범
        //return Screen.dpi * canvasSize.height / Screen.height / 160 * bannerAdSize.Height;
        return Screen.dpi * canvasSize.height / Screen.height / 120 * bannerAdSize.Height;
    }

    /// <summary>    
    /// 배너 요청
    /// (배너는 메인 진입 완료 시 요청)
    /// </summary>
    public void RequestBanner()
    {
        if (bannerView != null &&
            isLoadedBanner)
        {
            bannerView.Show();
            OnLoadedBanner(true);
            return;
        }

        string adUnitId;
        if (GameManager.Instance.GameSettings.IsAdLive)
        {
            #if UNITY_ANDROID
                adUnitId = androidBannerId;
            #elif UNITY_IPHONE
                adUnitId = iOSBannerId;
            #else
		        adUnitId = "unexpected_platform";
            #endif
        }
        else
        {
            #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/6300978111";
            #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/2934735716";
            #else
		        adUnitId = "unexpected_platform";
            #endif
        }

        // 배너 삭제
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        OnLoadedBanner(false);

        if (!GameManager.Instance.GameSettings.UseAd)
        {
            return;
        }

        bannerView = new BannerView(adUnitId, bannerAdSize, AdPosition.Top);
        bannerView.OnBannerAdLoaded += HandleBannerAdLoaded;
        bannerView.OnBannerAdLoadFailed += HandleBannerAdLoadFailed;
        bannerView.OnAdPaid += HandleAdPaid;
        bannerView.OnAdImpressionRecorded += HandleAdImpressionRecorded;
        bannerView.OnAdClicked += HandleAdClicked;
        bannerView.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpened;
        bannerView.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosed;

        AdRequest request = new AdRequest.Builder()
            .Build();
        bannerView.LoadAd(request);
    }

    public void RefreshBanner()
    {
        isLoadedBanner = false;
        RequestBanner();
    }

    public void ActiveBanner(bool active)
    {
        if (bannerView == null)
        {
            return;
        }

        if (active)
        {
            bannerView.Show();
        }
        else
        {
            bannerView.Hide();
        }
    }

    private void HandleBannerAdLoaded()
    {
        isLoadedBanner = true;
        OnLoadedBanner(true);
    }

    private void HandleBannerAdLoadFailed(LoadAdError error)
    {
        isLoadedBanner = false;
    }

    private void HandleAdPaid(AdValue adValue) { }
    private void HandleAdImpressionRecorded() { }
    private void HandleAdClicked() { }
    private void HandleAdFullScreenContentOpened() { }
    private void HandleAdFullScreenContentClosed() { }
    #endregion

    #region 메이크업 페이지 아이템 광고
    /// <summary>
    /// 메이크업 페이지 아이템 광고 요청
    /// </summary>
    private void RequestMakingItemVideo()
    {
        if (makingItemAd != null &&
            makingItemAd.CanShowAd())
        {
            return;
        }
        if (isLoadingMakingItemAd)
        {
            return;
        }
        isLoadingMakingItemAd = true;

        string adUnitId;
        if (GameManager.Instance.GameSettings.IsAdLive)
        {
            #if UNITY_ANDROID
                adUnitId = androidMakingItemId;
            #elif UNITY_IPHONE
                adUnitId = iOSMakingItemId;
            #else
                adUnitId = "unexpected_platform";
            #endif
        }
        else
        {
            #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/5224354917";
            #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
            #else
                adUnitId = "unexpected_platform";
            #endif
        }

        AdRequest request = new AdRequest.Builder()
            .Build();

        RewardedAd.Load(adUnitId, request, (RewardedAd rewardedAd, LoadAdError error) =>
        {
            isLoadingMakingItemAd = false;

            if (error != null ||
                rewardedAd == null)
            {
                return;
            }

            makingItemAd = rewardedAd;

            makingItemAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpenedMakingItem;
            makingItemAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosedMakingItem;
            makingItemAd.OnAdImpressionRecorded += HandleAdImpressionRecordedMakingItem;
            makingItemAd.OnAdClicked += HandleAdClickedMakingItem;
            makingItemAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailedMakingItem;
            makingItemAd.OnAdPaid += HandleAdPaidMakingItem;
        });
    }

    /// <summary>
    /// 광고 시청
    /// </summary>
    public void WatchVideoWithMakingItem(Action<AdsCallbackState> callback)
    {
        if (!GameManager.Instance.GameSettings.UseAd)
        {
            callback?.Invoke(AdsCallbackState.Success);
            return;
        }

        if (makingItemAd != null &&
            makingItemAd.CanShowAd())
        {
            this.callback = callback;
            makingItemAd.Show((Reward reward) =>
            {
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Complete();
                });
            });
        }
        else
        {
            // 광고 로딩 중 알림창 필요
            callback?.Invoke(AdsCallbackState.Loading);
            RequestMakingItemVideo();
        }
    }

    private void HandleAdFullScreenContentOpenedMakingItem()
    {
        isLockAppOpen = true;
        isOnceLockAppOpen = true;
    }

    private void HandleAdFullScreenContentClosedMakingItem()
    {
        isLockAppOpen = false;
        RequestMakingItemVideo();
    }

    private void HandleAdImpressionRecordedMakingItem() { }
    private void HandleAdClickedMakingItem() { }
    private void HandleAdFullScreenContentFailedMakingItem(AdError error) { }
    private void HandleAdPaidMakingItem(AdValue adValue) { }
    #endregion

    #region 옷입히기 페이지 아이템 광고
    /// <summary>
    /// 옷입히기 페이지 아이템 광고 요청
    /// </summary>
    private void RequestClothItemVideo()
    {
        if (clothItemAd != null &&
            clothItemAd.CanShowAd())
        {
            return;
        }
        if (isLoadingClothItemAd)
        {
            return;
        }
        isLoadingClothItemAd = true;

        string adUnitId;
        if (GameManager.Instance.GameSettings.IsAdLive)
        {
            #if UNITY_ANDROID
                adUnitId = androidClothItemId;
            #elif UNITY_IPHONE
                adUnitId = iOSClothItemId;
            #else
                adUnitId = "unexpected_platform";
            #endif
        }
        else
        {
            #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/5224354917";
            #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
            #else
                adUnitId = "unexpected_platform";
            #endif
        }

        AdRequest request = new AdRequest.Builder()
            .Build();

        RewardedAd.Load(adUnitId, request, (RewardedAd rewardedAd, LoadAdError error) =>
        {
            isLoadingClothItemAd = false;

            if (error != null ||
                rewardedAd == null)
            {
                return;
            }

            clothItemAd = rewardedAd;

            clothItemAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpenedClothItem;
            clothItemAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosedClothItem;
            clothItemAd.OnAdImpressionRecorded += HandleAdImpressionRecordedClothItem;
            clothItemAd.OnAdClicked += HandleAdClickedClothItem;
            clothItemAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailedClothItem;
            clothItemAd.OnAdPaid += HandleAdPaidClothItem;
        });
    }

    /// <summary>
    /// 광고 시청
    /// </summary>
    public void WatchVideoWithClothItem(Action<AdsCallbackState> callback)
    {
        if (!GameManager.Instance.GameSettings.UseAd)
        {
            callback?.Invoke(AdsCallbackState.Success);
            return;
        }

        if (clothItemAd != null &&
            clothItemAd.CanShowAd())
        {
            this.callback = callback;
            clothItemAd.Show((Reward reward) =>
            {
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    Complete();
                });
            });
        }
        else
        {
            // 광고 로딩 중 알림창 필요
            callback?.Invoke(AdsCallbackState.Loading);
            RequestClothItemVideo();
        }
    }

    private void HandleAdFullScreenContentOpenedClothItem()
    {
        isLockAppOpen = true;
        isOnceLockAppOpen = true;
    }

    private void HandleAdFullScreenContentClosedClothItem()
    {
        isLockAppOpen = false;
        RequestClothItemVideo();
    }

    private void HandleAdImpressionRecordedClothItem() { }
    private void HandleAdClickedClothItem() { }
    private void HandleAdFullScreenContentFailedClothItem(AdError error) { }
    private void HandleAdPaidClothItem(AdValue adValue) { }
    #endregion

    #region 기능 사용
    /// <summary>
    /// 기능 사용 광고 요청
    /// </summary>
    private void RequestFuncUse()
    {
        if (funcUseAd != null &&
            funcUseAd.CanShowAd())
        {
            return;
        }
        if (isLoadingFuncUseAd)
        {
            return;
        }
        isLoadingFuncUseAd = true;

        string adUnitId;
        if (GameManager.Instance.GameSettings.IsAdLive)
        {
            #if UNITY_ANDROID
                adUnitId = androidFuncUseId;
            #elif UNITY_IPHONE
                adUnitId = iOSFuncUseId;
            #else
                adUnitId = "unexpected_platform";
            #endif
        }
        else
        {
            #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/1033173712";
            #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/4411468910";
            #else
                adUnitId = "unexpected_platform";
            #endif
        }

        AdRequest request = new AdRequest.Builder()
            .Build();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd interstitialAd, LoadAdError error) =>
        {
            isLoadingFuncUseAd = false;

            if (error != null ||
                interstitialAd == null)
            {
                return;
            }

            funcUseAd = interstitialAd;

            funcUseAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpenedFuncUse;
            funcUseAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosedFuncUse;
            funcUseAd.OnAdImpressionRecorded += HandleAdImpressionRecordedFuncUse;
            funcUseAd.OnAdClicked += HandleAdClickedFuncUse;
            funcUseAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailedFuncUse;
            funcUseAd.OnAdPaid += HandleAdPaidFuncUse;
        });
    }

    /// <summary>
    /// 광고 시청
    /// </summary>
    public void WatchAdWithFuncUse(string key = null)
    {
        if (!GameManager.Instance.GameSettings.UseAd)
        {
            return;
        }

        bool isPass = false;
        if (!string.IsNullOrEmpty(key))
        {
            isPass = PlayerPrefs.GetInt(key, 0) == 0;
        }

        if (funcUseAd != null &&
            funcUseAd.CanShowAd())
        {
            if (isPass)
            {
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                funcUseAd.Show();
            }
        }
        else
        {
            RequestFuncUse();
        }
    }

    private void HandleAdFullScreenContentOpenedFuncUse()
    {
        isLockAppOpen = true;
        isOnceLockAppOpen = true;
    }

    private void HandleAdFullScreenContentClosedFuncUse()
    {
        isLockAppOpen = false;
        RequestFuncUse();
    }

    private void HandleAdImpressionRecordedFuncUse() { }
    private void HandleAdClickedFuncUse() { }
    private void HandleAdFullScreenContentFailedFuncUse(AdError error) { }
    private void HandleAdPaidFuncUse(AdValue adValue) { }
    #endregion

    #region 저장
    /// <summary>
    /// 저장 광고 요청
    /// </summary>
    private void RequestSave()
    {
        if (saveAd != null &&
            saveAd.CanShowAd())
        {
            return;
        }
        if (isLoadingSaveAd)
        {
            return;
        }
        isLoadingSaveAd = true;

        string adUnitId;
        if (GameManager.Instance.GameSettings.IsAdLive)
        {
            #if UNITY_ANDROID
                adUnitId = androidSaveId;
            #elif UNITY_IPHONE
                adUnitId = iOSSaveId;
            #else
                adUnitId = "unexpected_platform";
            #endif
        }
        else
        {
            #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/1033173712";
            #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/4411468910";
            #else
                adUnitId = "unexpected_platform";
            #endif
        }

        AdRequest request = new AdRequest.Builder()
            .Build();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd interstitialAd, LoadAdError error) =>
        {
            isLoadingSaveAd = false;

            if (error != null ||
                interstitialAd == null)
            {
                return;
            }

            saveAd = interstitialAd;

            saveAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpenedSave;
            saveAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosedSave;
            saveAd.OnAdImpressionRecorded += HandleAdImpressionRecordedSave;
            saveAd.OnAdClicked += HandleAdClickedSave;
            saveAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailedSave;
            saveAd.OnAdPaid += HandleAdPaidSave;
        });
    }

    /// <summary>
    /// 광고 시청
    /// </summary>
    public void WatchAdWithSave()
    {
        if (!GameManager.Instance.GameSettings.UseAd)
        {
            return;
        }

        if (saveAd != null &&
            saveAd.CanShowAd())
        {
            saveAd.Show();
        }
        else
        {
            RequestSave();
        }
    }

    private void HandleAdFullScreenContentOpenedSave()
    {
        isLockAppOpen = true;
        isOnceLockAppOpen = true;
    }

    private void HandleAdFullScreenContentClosedSave()
    {
        isLockAppOpen = false;
        RequestSave();
    }

    private void HandleAdImpressionRecordedSave() { }
    private void HandleAdClickedSave() { }
    private void HandleAdFullScreenContentFailedSave(AdError error) { }
    private void HandleAdPaidSave(AdValue adValue) { }
    #endregion
}
