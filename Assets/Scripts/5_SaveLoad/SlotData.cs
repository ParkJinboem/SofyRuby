using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClothData {
    public MenuSelector _clothCate;
    public int _cloth_resource_idx;
    public bool _isEnable;
}

[Serializable]
public class SlotData
{
    //public int id;
    //public string sceneIdx;
    //public int skinIdx;
    //public List<MENUSELECTOR> shirts;

    public int _id;
    public string _sceneName;
    public int _MainSceneIdx;
    public int _slotskinIdx;
    public int _sloteyeIdx;
    public int _slothairIdx;
    public int _slotBGIdx;
    public List<ClothData> _cloth;
}

[Serializable]
public class ClothLockData
{
   public int _iIdx;
   public bool _isLock;
}
[Serializable]
public class HairLockDate
{
    public int _Idx;
    public bool _isLock;
}
[Serializable]
public class Hair
{
    public List<HairLockDate> _hairData;
}

[Serializable]
public class Cloth
{
   public List<ClothLockData> _ShirtData;
   public List<ClothLockData> _PantsData;
   public List<ClothLockData> _DressData;
   public List<ClothLockData> _HatData;
   public List<ClothLockData> _EarringData;
   public List<ClothLockData> _GlassData;
   public List<ClothLockData> _ArmringData;
   public List<ClothLockData> _NecklessData;
   public List<ClothLockData> _ShoseData;
   public List<ClothLockData> _OuterData;
   public List<ClothLockData> _PetData;
   public List<ClothLockData> _SkirtData;
   public List<ClothLockData> _StockingData;
   public List<ClothLockData> _BagData;
    public List<ClothLockData> _BGData;
}
