using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : Activable
{
    private Transform leftWing;
    private Vector3 leftWingStartPosition;
    private Transform rightWing;
    private Vector3 rightWingStartPosition;
    [SerializeField] private float openingDistance;
    [SerializeField] private float openingSpeed;

    protected override void Awake()
    {
        base.Awake();
        leftWing = transform.GetChild(0);
        rightWing = transform.GetChild(1);
        leftWingStartPosition = leftWing.position;
        rightWingStartPosition = rightWing.position;
    }

    protected override void ActionOnUse()
    {
        isActive = !isActive;
    }

    public void Update()
    {
        float step = Time.deltaTime * openingSpeed;
        if (isActive)
        {
            if (Vector3.Distance(leftWing.position, rightWing.position) < openingDistance)
            {
                leftWing.Translate(-Vector3.right * step);
                rightWing.Translate(Vector3.right * step);
            }
        }
        else
        {
            leftWing.position = Vector3.MoveTowards(leftWing.position, leftWingStartPosition, step);
            rightWing.position = Vector3.MoveTowards(rightWing.position, rightWingStartPosition, step);
        }
    }
}
