using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelRotation : MonoBehaviour
{
    [SerializeField] private AnimationCurve tunnelAnimationCurve;
    private Vector3 maxPosition;

    [SerializeField] private float duration;
    [SerializeField] private float intensity;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("TAMERE");
        float elapsed = Time.time - startTime;
        float value = intensity * tunnelAnimationCurve.Evaluate(elapsed / duration);
        Debug.Log(value);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTrans = transform.GetChild(i);
            maxPosition = new Vector3(childTrans.position.x + 10, childTrans.position.y, childTrans.position.z);
            childTrans.Translate(Vector3.Lerp(childTrans.position, maxPosition, value));
        }
    }
}
