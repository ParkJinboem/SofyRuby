using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField]
    private bool isAdLive = false;
    public bool IsAdLive
    {
        get { return isAdLive; }
    }

    [SerializeField]
    private bool useAd = false;
    public bool UseAd
    {
        get { return useAd; }
    }

    [SerializeField]
    private string iOSAppStoreId;
    public string IOSAppStoreId
    {
        get { return iOSAppStoreId; }
    }
}