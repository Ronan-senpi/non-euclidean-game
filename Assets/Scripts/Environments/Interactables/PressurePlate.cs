using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Activable linkedObject;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Interactable")
        {
            PressPlate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Interactable")
        {
            QuitPlate();
        }
    }

    public void PressPlate()
    {
        if(linkedObject != null)
        {
            linkedObject.onActivate?.Invoke();
        }
        else
        {
            Debug.LogError("No object linked to " + name);
        }
    }

    public void QuitPlate()
    {
        if (linkedObject != null)
        {
            linkedObject.onDeactivate?.Invoke();
        }
        else
        {
            Debug.LogError("No object linked to " + name);
        }
    }
}
