using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevating : Activable
{
    [SerializeField] private float elevatingSpeed;
    [SerializeField] private GameObject elevatingPosition;
    [SerializeField] private Transform elevatingTransform;
    [SerializeField] private int numberOfActivationNeeded = 1;
    private int currentActivation = 0;

    private Vector3 startPosition;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
    }

    protected override void ActionOnUse(bool stateAction)
    {
        if (stateAction)
            currentActivation++;
        else
            currentActivation--;

        if (currentActivation >= numberOfActivationNeeded)
            isActive = true;
        else
            isActive = false;
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
