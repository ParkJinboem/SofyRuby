using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IntroScene : MonoBehaviour
{

    public void OnStartButton()
    {
        SceneManager.LoadScene(GameManager.MainSceneName);
    }

    
}
