using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingEssentialsObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> essentials = new List<GameObject>();

    private void Awake()
    {
        foreach(GameObject go in essentials)
        {
            Instantiate(go);
        }
    }

}
