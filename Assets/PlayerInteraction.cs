using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    public Transform cam;
    private Ray camRay;
    private RaycastHit hit;
    public Interactable hitObject;

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

                hitObject = hit.collider.gameObject.GetComponent<Interactable>();

            }
            else if (hitObject != null)
            {
                hitObject = null;
            }
        }
        else if (hitObject != null)
        {
            hitObject = null;
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (hitObject != null)
        {
            if (Vector3.Distance(hitObject.transform.position, cam.position) > maxDistance)
            {
                hitObject = null;
            }

        }

        if (Input.GetKeyDown(KeyCode.F) && hitObject != null)
        {
            Debug.Log("Press");
            hitObject.GetComponent<Interactable>().onPlayerInteract?.Invoke();
        }
    }
}
