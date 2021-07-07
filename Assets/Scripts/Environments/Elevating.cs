using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevating : Activable
{
    [SerializeField] private float elevatingSpeed;
    [SerializeField] private GameObject elevatingPosition;
    [SerializeField] private Transform elevatingTransform;
    private Vector3 startPosition;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
    }

    protected override void ActionOnUse()
    {
        isActive = !isActive;
    }

    private void Update()
    {
        float step = Time.deltaTime * elevatingSpeed;
        if (isActive)
        {
            elevatingTransform.position = Vector3.MoveTowards(elevatingTransform.position, elevatingPosition.transform.position, step);
        }
        else
        {
            elevatingTransform.position = Vector3.MoveTowards(elevatingTransform.position, startPosition, step);
        }
    }
}
