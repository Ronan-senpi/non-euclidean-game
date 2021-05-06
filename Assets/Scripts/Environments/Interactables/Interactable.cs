using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public UnityEvent onPlayerInteract = new UnityEvent();
    [HideInInspector] public UnityEvent onPlayerLooking = new UnityEvent();
    [HideInInspector] public UnityEvent onPlayerStopLooking = new UnityEvent();

    private Outline outline;

    protected virtual void Awake()
    {
        outline = GetComponent<Outline>();
        onPlayerInteract.AddListener(ActionOnUse);
        onPlayerLooking.AddListener(HighlightInteraction);
        onPlayerStopLooking.AddListener(StopInteraction);
    }


    protected virtual void ActionOnUse() { }

    protected void HighlightInteraction()
    {
        if (outline != null)
            outline.enabled = true;
    }

    protected void StopInteraction()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
