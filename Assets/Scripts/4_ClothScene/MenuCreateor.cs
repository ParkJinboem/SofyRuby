#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public enum MenuSelector
{
    None,
    Shirts,
    Pants,
    Dress,
    Hat,
    Earring,
    Glass,
    Armring,
    Neckless,
    Shose,
    Other,
    Pet,
    AllA,
    AllB,
    Background,

    Hair = 1000
}

public class MenuCreateor : MonoBehaviour
{
    public void SetupResizeToImage()
    {
        Sprite sp = GetComponent<Image>().sprite;
        Vector2 size = new Vector2(sp.rect.width, sp.rect.height);
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetupClothMenu()
    {
        ClothMenu[] clothMenus = GetComponentsInChildren<ClothMenu>();
        for (int i = 0; i < clothMenus.Length; i++)
        {
            ClothMenu clothMenu = clothMenus[i];
            // 아이콘 이미지
            clothMenu.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Sprites/ClothScene/MenuIcon/{0:D2}", (int)clothMenu.ms));
            // 버튼 이미지
            Sprite spSelect = Resources.Load<Sprite>(string.Format("Sprites/ClothScene/MenuIconSelect/{0:D2}", (int)clothMenu.ms));
            SpriteState ss = clothMenu.GetComponent<Button>().spriteState;
            ss.pressedSprite = spSelect;
            ss.disabledSprite = spSelect;
            clothMenu.GetComponent<Button>().spriteState = ss;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MenuCreateor))]
public class MenuSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MenuCreateor generator = (MenuCreateor)target;
        if (GUILayout.Button("Setup Resize To Image"))
        {
            generator.SetupResizeToImage();
        }
        if (GUILayout.Button("Setup Cloth Menu"))
        {
            generator.SetupClothMenu();
        }
    }
}
#endif