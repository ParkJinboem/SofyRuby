using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GirlHolder : MonoBehaviour
{
    public RawImage _Mask_Cheek;
    public RawImage _Mask_EyeBrow;
    public RawImage _Mask_Shadow;
    public RawImage _Mask_Lipstick;

    public GameObject _Pimples;
    public Image _Hair;
    public Image _Eye;
    public Image _Skin;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == GameManager.ClothSceneName)
        {
            WorldMgr.GetInst().SetClothSceneJuju();
        }
    }
}
