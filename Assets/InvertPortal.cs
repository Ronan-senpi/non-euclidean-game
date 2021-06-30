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
            portal3.enabled = true;
            portal2.enabled = false;
            portal1._LinkedPortal = portal3;
            portal2._LinkedPortal = null;
            portal3._LinkedPortal = portal1;
            portal3.ResetViewtTexture();
            portal2.ResetViewtTexture();
            portal1.ResetViewtTexture();
        }
        else
        {
            portal2.enabled = true;
            portal3.enabled = false;
            portal1._LinkedPortal = portal2;
            portal3._LinkedPortal = null;
            portal2._LinkedPortal = portal1;
            portal2.ResetViewtTexture();
            portal1.ResetViewtTexture();
            portal3.ResetViewtTexture();
        }
    }

}
