using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMovable : Interactable
{
    [SerializeField] private Transform takenPosition;
    [SerializeField] private float moveForce = 250;
    private bool taken = false;
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void ActionOnUse()
    {
        if (!taken)
        {
            gameObject.layer = 10;
            PlayerInteraction.Instance.ObjectInHand = this;
            transform.position = takenPosition.position;
            transform.SetParent(takenPosition);
            taken = true;
            rb.useGravity = false;
            rb.drag = 10;
            //rb.isKinematic = true;
        }
        else
        {
            gameObject.layer = 0;
            rb.useGravity = true;
            rb.drag = 1;
            //rb.isKinematic = false;
            transform.SetParent(null);
            taken = false;
        }
    }

    private void Update()
    {
        if (taken)
        {
            if(Vector3.Distance(transform.position, takenPosition.position) > 0.3f)
            {
                Vector3 moveDirection = (takenPosition.position - transform.position).normalized;
                rb.AddForce(moveDirection * moveForce);
            }
        }
    }
}
