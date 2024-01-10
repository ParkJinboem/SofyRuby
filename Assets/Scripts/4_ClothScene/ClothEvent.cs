using System;

public class ClothEvent
{
    public enum SaveLoadState { None, Open, Close }
    public delegate void SaveLoadHandler(SaveLoadState saveLoadState);
    public static event SaveLoadHandler OnSaveLoad;
    public static void SaveLoad(SaveLoadState saveLoadState)
    {
        OnSaveLoad?.Invoke(saveLoadState);
    }

    public delegate void ClickMenuHandler(ClothMenu clothMenu);
    public static event ClickMenuHandler OnClickMenu;
    public static void ClickMenu(ClothMenu clothMenu)
    {
        OnClickMenu?.Invoke(clothMenu);
    }

    public delegate void WearClothHandler(ClothMenu clothMenu, int idx);
    public static event WearClothHandler OnWearCloth;
    public static void WearCloth(ClothMenu clothMenu, int idx)
    {
        OnWearCloth?.Invoke(clothMenu, idx);
    }

    public delegate void ClothLoadedHandler();
    public static event ClothLoadedHandler OnClothLoaded;
    public static void ClothLoaded()
    {
        OnClothLoaded?.Invoke();
    }
}
