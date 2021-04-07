using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : Activable
{
    private bool opened = false;

    protected override void ActionOnUse()
    {
        if (!opened)
        {
            transform.Rotate(Vector3.up, 90);
            opened = true;
        }
    }

    protected override void ActionOnDeactivate()
    {
        if (opened)
        {
            transform.Rotate(Vector3.up, -90);
            opened = false;
        }
    }
}
