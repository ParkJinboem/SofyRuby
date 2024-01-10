using UnityEngine;

public class OpenStore : MonoBehaviour
{
    public OpenStoreAlertView openStoreAlertView;

    public string androidId;
    public string iOSId;

    public void ShowAlert()
    {
        if (openStoreAlertView != null)
        {
            openStoreAlertView.Show(this);
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        #if UNITY_ANDROID
                    Application.OpenURL(string.Format("market://details?id={0}", androidId));
        #elif UNITY_IOS
                Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/id{0}", iOSId));
        #endif
    }
}
