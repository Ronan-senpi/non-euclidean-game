using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFog : MonoBehaviour
{
    [SerializeField]
    private Material FogMat;
    public float overalFogDensity;
    private void Start()
    {
        overalFogDensity = FogMat.GetFloat("OveralFogDensity");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            FogMat.SetFloat("OveralFogDensity", 0);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            FogMat.SetFloat("OveralFogDensity", overalFogDensity);

    }
}
