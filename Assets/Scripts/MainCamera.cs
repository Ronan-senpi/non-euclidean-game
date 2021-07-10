using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera instance;
    public static MainCamera Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MainCamera>();
            return instance;
        }
    }
    private Portal[] portals;

    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
    }

    void LateUpdate()
    {

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender();
        }
        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i].enabled)
            {
                Debug.Log(portals[i].gameObject.name);
                portals[i].Render();

            }
        }

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender();
        }
    }

    public void SearchAllPortals()
    {
        Debug.Log("RELOAD PORTALS");
        portals = FindObjectsOfType<Portal>();
    }

}