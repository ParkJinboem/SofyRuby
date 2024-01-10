using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteBtn_Checker : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<MakingStairBtn>().SetCompletePref(this.gameObject);
        //this.gameObject.SetActive(false);
    }
}
