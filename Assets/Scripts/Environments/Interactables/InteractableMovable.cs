using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMovable : Interactable
{
    [SerializeField] private Transform heldPosition;
    [SerializeField] private float moveForce;
    [SerializeField] private float dragForce;
    [SerializeField] private float distanceMax;
    private int objectFreeLayer;
    private int objectHeldLayer;

    private bool taken = false;
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        objectFreeLayer = LayerMask.NameToLayer("Default");
        objectHeldLayer = LayerMask.NameToLayer("ObjectHeld");
        rb = GetComponent<Rigidbody>();
    }

    protected override void ActionOnUse()
    {
        if (!taken)
        {
            gameObject.layer = objectHeldLayer;
            PlayerInteraction.Instance.ObjectInHand = this;
            transform.position = heldPosition.position;
            transform.SetParent(heldPosition);
            taken = true;
            rb.useGravity = false;
            rb.drag = dragForce;
            rb.freezeRotation = true;
            //rb.isKinematic = true;
        }
        else
        {
            gameObject.layer = objectFreeLayer;
            rb.useGravity = true;
            rb.drag = 1;
           // rb.isKinematic = false;
            transform.SetParent(null);
            taken = false;
            rb.freezeRotation = false;
        }
    }

    private void Update()
    {
        if (taken)
        {
            transform.rotation = heldPosition.rotation;
            if (Vector3.Distance(transform.position, heldPosition.position) > distanceMax)
            {
                Vector3 moveDirection = (heldPosition.position - transform.position).normalized;
                rb.AddForce(moveDirection * moveForce);
            }
        }
    }
}
