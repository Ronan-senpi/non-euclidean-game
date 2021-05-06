using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Activable linkedObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable" || other.gameObject.tag == "Player")
            PressPlate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Interactable" || other.gameObject.tag == "Player")
            PressPlate();
    }

    public void PressPlate()
    {
        if (linkedObject != null)
            linkedObject.onStateChange?.Invoke();
        else
            Debug.LogError("No object linked to " + name);
    }
}
