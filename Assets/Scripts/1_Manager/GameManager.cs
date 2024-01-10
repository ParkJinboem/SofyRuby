using UnityEngine;
using GoogleMobileAds.Api;
using System;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    // statics
    public const string IntroSceneName = "0_IntroScene";
    public const string MainSceneName = "1_MainScene";
    public const string ClothSceneName = "2_ClothScene";

    [SerializeField]
    private GameSettings gameSettings;
    public GameSettings GameSettings
    {
        get { return gameSettings; }
    }

    private Action initAdsCallback;

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

    private void Start()
    {
        RequestTrackingAuthorizationPlugin.OnRequestTA += HandlerRequestTA;
    }

    private void OnDestroy()
    {
        RequestTrackingAuthorizationPlugin.OnRequestTA -= HandlerRequestTA;
    }

    public void InitAds(Action callback)
    {
        this.initAdsCallback = callback;

        RequestTrackingAuthorizationPlugin.RequestTrackingAuthorization();
    }

    private void HandlerRequestTA(bool isAgree)
    {
        GatherConsent(agree =>
        {
            if (agree)
            {
                MobileAds.Initialize(initStatus =>
                {
                    AdsManager.Instance.InitAds();
                    initAdsCallback?.Invoke();
                });
            }
            else
            {
                initAdsCallback?.Invoke();
            }
        });
    }

    private void GatherConsent(Action<bool> complete)
    {
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        ConsentInformation.Update(request, consentError =>
        {
            if (consentError != null)
            {
                complete(false);
                UnityEngine.Debug.LogError(consentError);
                return;
            }

            if (ConsentInformation.CanRequestAds())
            {
                complete(true);
                return;
            }
            
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                    complete(false);
                    UnityEngine.Debug.LogError(consentError);
                    return;
                }

                complete(ConsentInformation.CanRequestAds());
            });
        });
    }
}
