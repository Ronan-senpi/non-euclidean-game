using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public UnityEvent onPlayerInteract = new UnityEvent();

    protected virtual void Awake()
    {
        onPlayerInteract.AddListener(ActionOnUse);
    }


    protected virtual void ActionOnUse() {
        Debug.Log("INTERACT");
    }
}
