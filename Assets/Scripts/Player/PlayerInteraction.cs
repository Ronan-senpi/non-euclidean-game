using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private static PlayerInteraction instance;
    public static PlayerInteraction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInteraction>();
            }
            return instance;
        }
    }


    [SerializeField] private float maxDistance;
    private Transform cam;
    private Ray camRay;
    private RaycastHit hit;
    public Interactable interactableObject;
    private Interactable objectInHand;
    public Interactable ObjectInHand
    {
        set { objectInHand = value; }
    }
    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        camRay = new Ray(cam.position, cam.forward);

        if (Physics.Raycast(camRay, out hit, maxDistance))
        {
            if (hit.collider.gameObject.tag == "Interactable")
            {
                interactableObject = hit.collider.gameObject.GetComponent<Interactable>();
                interactableObject.onPlayerLooking?.Invoke();
            }
            else if (interactableObject != null)
            {
                interactableObject.onPlayerStopLooking?.Invoke();
                interactableObject = null;

            }
        }
        else if (interactableObject != null)
        {
            interactableObject.onPlayerStopLooking?.Invoke();
            interactableObject = null;
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (interactableObject != null)
        {
            if (Vector3.Distance(interactableObject.transform.position, cam.position) > maxDistance)
            {
                interactableObject = null;
            }
        }

        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Input Interaction");
            if (interactableObject != null)
            {
                Debug.Log("Start Interacting");
                interactableObject.onPlayerInteract?.Invoke();
            }
            else if (objectInHand != null)
            {
                objectInHand.onPlayerInteract?.Invoke();
                objectInHand = null;
            }
        }
    }
}
