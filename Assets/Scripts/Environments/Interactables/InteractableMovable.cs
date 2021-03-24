using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMovable : Interactable
{
    [SerializeField] private Transform takenPosition;
    private bool taken = false;

    protected override void ActionOnUse()
    {
        if (!taken)
        {
            PlayerInteraction.Instance.ObjectInHand = this;
            transform.position = takenPosition.position;
            transform.SetParent(takenPosition);
            taken = true;
        }
        else
        {
            transform.SetParent(null);
            taken = false;
        }
    }


}
