using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RdmPortalController : MonoBehaviour
{
    private static RdmPortalController instance;
    public static RdmPortalController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RdmPortalController>();
            }
            return instance;
        }
    }

    [SerializeField]
    public int nbPassagesBeforeDestination = 3;
    public int currentPassagesBeforeDestination { get; private set; } = 0;

    [SerializeField]
    Portal destination;


    [SerializeField]
    List<GameObject> portalsContainers;

    List<GameObject> portals = new List<GameObject>();

    private void Awake()
    {
        //Init la liste des portails
        foreach (GameObject item in portalsContainers)
            for (int i = 0; i < item.transform.childCount; i++)
                portals.Add(item.transform.GetChild(i).gameObject);
    }

    public Portal GetPortal(int index)
    {
        if (currentPassagesBeforeDestination >= nbPassagesBeforeDestination)
        {
            return destination;
        }
        if (portals.Count > index)
        {
            ++currentPassagesBeforeDestination;
            Debug.Log("currentPassagesBeforeDestination" + currentPassagesBeforeDestination);
            return portals[index].GetComponent<Portal>();
        }
        return null;
    }

    public Portal GetRdmPortal()
    {
        return this.GetPortal(Random.Range(0, portals.Count));
    }
}
