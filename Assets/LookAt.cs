using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField]
    Transform whoLook;
    bool canLook = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canLook = true;
        }
    }
    private void Update()
    {
        if (canLook)
            whoLook.LookAt(Camera.main.transform);
    }
}
