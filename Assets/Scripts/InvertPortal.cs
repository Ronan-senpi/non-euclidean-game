using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertPortal : Activable
{
    [SerializeField] private Portal portal1;
    [SerializeField] private Portal portal2;
    [SerializeField] private Portal portal3;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void ActionOnUse()
    {

        if (portal1._LinkedPortal == portal2)
        {
            portal1.SwitchExit(portal3);
        }
        else
        {
            portal1.SwitchExit(portal2);
        }
    }

}
