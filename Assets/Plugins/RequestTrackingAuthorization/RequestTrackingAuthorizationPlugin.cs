using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class RequestTrackingAuthorizationPlugin : MonoBehaviour
{
    #if UNITY_IOS
        [DllImport("__Internal")]
        public static extern void _RequestTrackingAuthorization(DelegateMessage delegateMessage);

        [DllImport("__Internal")] 
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);            
        }
    #endif

    public delegate void RequestTAHandler(bool isAgree);
    public static event RequestTAHandler OnRequestTA;
    public static void RequestTA(bool isAgree)
    {
        OnRequestTA?.Invoke(isAgree);
    }

    public delegate void DelegateMessage(bool isAgree);

    [MonoPInvokeCallback(typeof(DelegateMessage))]
    private static void delegateMessageReceived(bool isAgree)
    {
        #if !UNITY_EDITOR && UNITY_IOS
            SetAdvertiserTrackingEnabled(isAgree);
        #endif

        RequestTA(isAgree);
    }

    public static void RequestTrackingAuthorization()
    {
        #if !UNITY_EDITOR && UNITY_IOS
            _RequestTrackingAuthorization(delegateMessageReceived);
        #else
            delegateMessageReceived(true);
        #endif
    }
}