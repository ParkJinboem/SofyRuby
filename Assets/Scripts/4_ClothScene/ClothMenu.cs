using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ClothMenu : MonoBehaviour
{
    public enum CheckPosition
    {
        None,
        Top,
        Bottom,
        All,
    }

    public MenuSelector ms;
    public Transform trClothIcon;
    public Sprite[] spClothIcons;    
    public List<MenuSelector> disableClothes;
    public CheckPosition checkPosition;
    public bool useRemove = true;

    private Button btn;

    [Header("Default Wear")]
    [SerializeField] bool isDefaultWear = false;    
    [SerializeField] int defaultIdx = 1;

    public bool IsDefaultWear
    {
        get { return isDefaultWear && !(defaultIdx == 1 && useRemove); }
    }

    public int DefaultIdx
    {
        get { return defaultIdx; }
    }

    private void Awake()
    {
        ClothEvent.OnClickMenu += HandlerClickMenu;
    }

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        spClothIcons = MakingMgr.GetInst().SpriteLoaderIcon(ms);
    }

    private void OnDestroy()
    {
        ClothEvent.OnClickMenu -= HandlerClickMenu;
    }

    public void OnClick()
    {
        ClothEvent.ClickMenu(this);
    }

    private void HandlerClickMenu(ClothMenu clothMenu)
    {
        btn.interactable = clothMenu.ms != ms;
    }
}
