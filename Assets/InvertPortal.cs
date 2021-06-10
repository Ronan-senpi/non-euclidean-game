using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertPortal : Activable
{
    [SerializeField] private Portal portalIn;
    [SerializeField] private Portal portalOut1;
    [SerializeField] private Portal portalOut2;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void ActionOnUse()
    {
        if (portalIn._LinkedPortal == portalOut1)
        {
            Debug.Log("AHAHAH");
            portalIn._LinkedPortal = portalOut2;
        }
        else
        {
            Debug.Log("OHOHOHO");
            portalIn._LinkedPortal = portalOut1;
        }
    }

}
