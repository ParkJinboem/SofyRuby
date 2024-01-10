using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PrefBtn_EventFunc : MonoBehaviour
{
    [SerializeField] UnityEvent _ev = null;
    EventTrigger _trigger;

    public UnityEvent Get_ev()
    {
        return _ev;
    }
    

    
}
