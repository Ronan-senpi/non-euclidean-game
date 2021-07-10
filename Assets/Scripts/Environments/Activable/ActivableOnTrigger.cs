using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableOnTrigger : Activable
{
    [SerializeField] private GameObject go;

    protected override void ActionOnUse()
    {
        go.SetActive(true);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ActionOnUse();
        }
    }

}
