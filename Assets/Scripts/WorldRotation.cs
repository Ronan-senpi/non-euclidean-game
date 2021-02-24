using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotation : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private GameObject playerCopy;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject empty = GameObject.CreatePrimitive(PrimitiveType.Cube);
            empty.transform.position = playerTransform.position; 
            
        }
    }
}
