using UnityEngine;
using System.Collections;
#if UNITY_ANDROID
using Google.Play.Review;
#endif

public class AppReviewManager : MonoBehaviour
{
    public static AppReviewManager Instance = null;

    public int[] checkCounts;

    public int maxCheckCount;
    public int appOpenCount;
    public bool isWriteReview;

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
        appOpenCount = PlayerPrefs.GetInt("AppOpenCount", 0);
        maxCheckCount = checkCounts[checkCounts.Length - 1];
        isWriteReview = PlayerPrefs.GetInt("isWriteReview", 0) == 1;
    }

    private void Reset()
    {
        PlayerPrefs.SetInt("AppOpenCount", 1);
        PlayerPrefs.SetInt("isWriteReview", 0);
    }

    public bool CheckAppReview()
    {
        // 카운트 증가
        appOpenCount += 1;
        if (appOpenCount > maxCheckCount)
        {
            appOpenCount = maxCheckCount;
        }
        PlayerPrefs.SetInt("AppOpenCount", appOpenCount);

        bool isShow = false;

        if (!isWriteReview)
        {
            // 최대치
            if (appOpenCount == maxCheckCount)
            {
                isWriteReview = true;
                PlayerPrefs.SetInt("isWriteReview", 1);
            }

            // 앱 리뷰 띄울지 확인
            for (int i = 0; i < checkCounts.Length; i++)
            {
                if (appOpenCount == checkCounts[i])
                {
                    isShow = true;
                    break;
                }
            }
        }

        return isShow;
    }

    public void ShowAppReview()
    {
        isWriteReview = true;
        PlayerPrefs.SetInt("isWriteReview", 1);
        #if UNITY_ANDROID                            
            StartCoroutine(ICheckReview());
        #elif UNITY_IPHONE                            
            Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/{0}", GameManager.Instance.GameSettings.IOSAppStoreId));
        #endif
    }

    #if UNITY_ANDROID
    private IEnumerator ICheckReview()
    {
        ReviewManager reviewManager = new ReviewManager();
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }

        var launchFlowOperation = reviewManager.LaunchReviewFlow(requestFlowOperation.GetResult());
        yield return launchFlowOperation;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
    #endif
}
