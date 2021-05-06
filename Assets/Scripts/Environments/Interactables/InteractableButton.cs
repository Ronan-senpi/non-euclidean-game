using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : Interactable
{
    [SerializeField] private Activable linkedObject;

    protected override void ActionOnUse()
    {
        linkedObject.onStateChange?.Invoke();
    }
}
