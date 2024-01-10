using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpriteSelectorIDX
{
    TOOL =0,
    SKIN,
    MASK,
    LENS,
    EYES,
    BACKGROUND,
    JUJUHAIR,
    EYESHADOWCOLOR,
    EYESHADOW,
    LIPSTICKCOLOR,
    LIPSTICK,
    CHEEK,
    CHEEKCOLOR,
    EYEBROW,
    EYEBROWCOLOR,
    HAIR,
    HAIRCOLOR,
    CLOTHSCENEMENU,
    CLOTHSCENEMENUS,
    FULLSKIN
}

[Serializable]
public class ClothPath
{
    public MenuSelector ms;
    public string clothPath;
    public string clothIconPath;
}

/// <summary>
/// 리소스 로드 편하게 쓰기 위한 클래스
/// </summary>
public class MakingMgr : MonoBehaviour
{
    #region SingleTon
    private static MakingMgr _inst;
    public static MakingMgr GetInst()
    {
        if (_inst == null) { Debug.LogError("Making Manager Instance is Null"); return null; }
        else return _inst;
    }

    [SerializeField] List<ClothPath> clothPaths;

    private void Awake()
    {
        if (_inst != null)
        {
            Destroy(gameObject);
        }
        else _inst = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region 화장씬 리소스 루트
    public const string _SkinPath = "Sprites/MakingScene/Skin";
    public const string _ToolPath = "Sprites/MakingScene/MenuHolder";
    public const string _MaksPath = "";
    public const string _LensPath = "Sprites/MakingScene/FaceHolder/EyeLensColor";
    public const string _EyePath = "Sprites/MakingScene/FaceHolder/EyeLens";
    public const string _BGPath = "Sprites/MakingScene/Bg";
    public const string _HairPaht = "Sprites/MakingScene/jujuHair";
    public const string _EyeShadowPath = "Sprites/MakingScene/FaceHolder/EyeShadow";
    public const string _UsingEyeShadowColor = "Sprites/MakingScene/FaceHolder/EyeShadowColor";
    public const string _LipStick = "Sprites/MakingScene/FaceHolder/Lips";
    public const string _LipStickColor = "Sprites/MakingScene/FaceHolder/LipsColor";
    public const string _CheekPath = "Sprites/MakingScene/FaceHolder/Cheek";
    public const string _CheekColorPath = "Sprites/MakingScene/FaceHolder/CheekColor";
    public const string _EyeBrowPath = "Sprites/MakingScene/FaceHolder/EyeBrow";
    public const string _EyeBrowColorPath = "Sprites/MakingScene/FaceHolder/EyeBrowColor";
    public const string _HairColorPath = "Sprites/MakingScene/FaceHolder/Hair";
    public const string _HairColorSelectorPath = "Sprites/MakingScene/FaceHolder/HairColor";
    public const string _ClothSceneMenuIcon_NS = "Sprites/ClothScene/MenuIcon";
    public const string _ClothSceneMenuIcon_S = "Sprites/ClothScene/MenuIconSelect";
    public const string _FullSkin = "Sprites/ClothScene/FullSkin";
    #endregion

    public string GetLensPath() { return _LensPath; }

    public Sprite[] SpriteLoader(SpriteSelectorIDX _idx)
    {
        switch (_idx)
        {
            case SpriteSelectorIDX.TOOL:
                return Resources.LoadAll<Sprite>(_ToolPath);
            case SpriteSelectorIDX.SKIN:
                return Resources.LoadAll<Sprite>(_SkinPath);
            case SpriteSelectorIDX.MASK:
                return Resources.LoadAll<Sprite>(_MaksPath);
            case SpriteSelectorIDX.LENS:
                return Resources.LoadAll<Sprite>(_LensPath);
            case SpriteSelectorIDX.EYES:
                return Resources.LoadAll<Sprite>(_EyePath);
            case SpriteSelectorIDX.BACKGROUND:
                return Resources.LoadAll<Sprite>(_BGPath);
            case SpriteSelectorIDX.JUJUHAIR:
                return Resources.LoadAll<Sprite>(_HairPaht);
            case SpriteSelectorIDX.EYESHADOWCOLOR:
                return Resources.LoadAll<Sprite>(_UsingEyeShadowColor);
            case SpriteSelectorIDX.EYESHADOW:
                return Resources.LoadAll<Sprite>(_EyeShadowPath);
            case SpriteSelectorIDX.LIPSTICK:
                return Resources.LoadAll<Sprite>(_LipStick);
            case SpriteSelectorIDX.LIPSTICKCOLOR:
                return Resources.LoadAll<Sprite>(_LipStickColor);
            case SpriteSelectorIDX.CHEEK:
                return Resources.LoadAll<Sprite>(_CheekPath);
            case SpriteSelectorIDX.CHEEKCOLOR:
                return Resources.LoadAll<Sprite>(_CheekColorPath);
            case SpriteSelectorIDX.EYEBROW:
                return Resources.LoadAll<Sprite>(_EyeBrowPath);
            case SpriteSelectorIDX.EYEBROWCOLOR:
                return Resources.LoadAll<Sprite>(_EyeBrowColorPath);
            case SpriteSelectorIDX.HAIR:
                return Resources.LoadAll<Sprite>(_HairColorPath);
            case SpriteSelectorIDX.HAIRCOLOR:
                return Resources.LoadAll<Sprite>(_HairColorSelectorPath);
            case SpriteSelectorIDX.CLOTHSCENEMENU:
                return Resources.LoadAll<Sprite>(_ClothSceneMenuIcon_NS);
            case SpriteSelectorIDX.CLOTHSCENEMENUS:
                return Resources.LoadAll<Sprite>(_ClothSceneMenuIcon_S);
            case SpriteSelectorIDX.FULLSKIN:
                return Resources.LoadAll<Sprite>(_FullSkin);
            default:
                return null;
        }
    }

    public Sprite SpriteLoader(MenuSelector ms, int number)
    {
        ClothPath clothPath = clothPaths.Find(x => x.ms == ms);
        if (clothPath != null)
        {
            return Resources.Load<Sprite>(string.Format(clothPath.clothPath, number));
        }
        else
        {
            return null;
        }
    }

    public Sprite[] SpriteLoaderIcon(MenuSelector ms)
    {
        ClothPath clothPath = clothPaths.Find(x => x.ms == ms);
        if (clothPath != null)
        {
            return Resources.LoadAll<Sprite>(clothPath.clothIconPath);
        }
        else
        {
            return null;
        }
    }

    //public Transform SpriteLoader(MenuSelector ms, int number)
    //{
    //    ClothPath clothPath = clothPaths.Find(x => x.ms == ms);
    //    if (clothPath != null)
    //    {
    //        return Resources.Load<Sprite>(string.Format(clothPath.clothPath, number));
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
}