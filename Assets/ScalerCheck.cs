using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerCheck : MonoBehaviour
{
    [SerializeField]
    Vector3 scaleCheck;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (other.transform.localScale.x != scaleCheck.x || other.transform.localScale.y != scaleCheck.y|| other.transform.localScale.z != scaleCheck.z)
            {
                other.transform.localScale = scaleCheck;
            }
        }
    }
}
