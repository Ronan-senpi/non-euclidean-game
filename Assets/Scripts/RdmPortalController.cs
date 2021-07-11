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
    GameObject portalsLeftContainers;
    [SerializeField]
    GameObject portalsRightContainers;

    List<Portal> portalsLeft = new List<Portal>();
    List<Portal> portalsRight = new List<Portal>();

    private void Start()
    {
        for (int i = 0; i < portalsLeftContainers.transform.childCount; i++)
        {
            Portal p;
            if (portalsLeftContainers.transform.GetChild(i).TryGetComponent(out p))
            {
                p.portalLeftRight = PortalLeftRight.Left;
                portalsLeft.Add(p);
            }
        }

        for (int i = 0; i < portalsRightContainers.transform.childCount; i++)
        {
            Portal p;
            if (portalsRightContainers.transform.GetChild(i).TryGetComponent(out p))
            {
                p.portalLeftRight = PortalLeftRight.Right;
                portalsRight.Add(p);
            }
        }
    }

    public Portal GetRdmPortal(PortalLeftRight currentPortal)
    {
        List<Portal> portals = currentPortal == PortalLeftRight.Left ? portalsRight : portalsLeft;
        int index = Random.Range(0, portals.Count);
        if (currentPassagesBeforeDestination >= nbPassagesBeforeDestination)
        {
            return destination;
        }
        if (portals.Count > index)
        {
            ++currentPassagesBeforeDestination;
            return portals[index].GetComponent<Portal>();
        }
        return null;
    }


}
