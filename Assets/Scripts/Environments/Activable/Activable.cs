using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Activable : MonoBehaviour
{
    [HideInInspector] public UnityEvent<bool> onStateChange = new UnityEvent<bool>();
    protected bool isActive;

    protected virtual void Awake()
    {
        onStateChange.AddListener(ActionOnUse);
    }

    protected virtual void ActionOnUse(bool action = false) { }
}
