using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Activable : MonoBehaviour
{
    [HideInInspector] public UnityEvent onStateChange = new UnityEvent();
    protected bool isActive;

    protected virtual void Awake()
    {
        onStateChange.AddListener(ActionOnUse);
    }

    protected virtual void ActionOnUse() { }
}
