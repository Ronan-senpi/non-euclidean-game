using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Activable : MonoBehaviour
{
    [HideInInspector] public UnityEvent onActivate = new UnityEvent();
    [HideInInspector] public UnityEvent onDeactivate = new UnityEvent();

    private void Awake()
    {
        onActivate.AddListener(ActionOnUse);
        onDeactivate.AddListener(ActionOnDeactivate);
    }

    protected virtual void ActionOnUse() { }
    protected virtual void ActionOnDeactivate() { }
}
