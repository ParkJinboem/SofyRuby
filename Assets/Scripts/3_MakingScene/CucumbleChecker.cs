using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CucumbleChecker : MonoBehaviour
{
    public void Show()
    {
        foreach (CucumbleRemover cr in gameObject.GetComponentsInChildren<CucumbleRemover>())
        {
            cr.Clear();
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
