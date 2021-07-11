using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private List<Activable> linkedObjects = new List<Activable>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable" || other.gameObject.tag == "Player")
            PressPlate(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Interactable" || other.gameObject.tag == "Player")
            PressPlate(false);
    }

    public void PressPlate(bool action)
    {
        if (linkedObjects != null)
        {
            for(int i = 0; i < linkedObjects.Count; i++)
                linkedObjects[i].onStateChange?.Invoke(action);
        }
        else
            Debug.LogError("No object linked to " + name);
    }
}
