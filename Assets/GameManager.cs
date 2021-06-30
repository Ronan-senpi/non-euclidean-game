using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    [SerializeField] private Volume postprocess;
    [SerializeField] private AnimationCurve fadeCurve;

    [SerializeField] private float duration;
    [SerializeField] private float intensity;

    private float currentValue;
    private float startTime;

    private bool activateAberration;
    private ChromaticAberration aberration;

    private void Awake()
    {
        postprocess.profile.TryGet(out aberration);

    }

    private void Update()
    {
        if (activateAberration)
        {
            float elapsed = Time.time - startTime;
            float value = intensity * fadeCurve.Evaluate(elapsed / duration);
            aberration.intensity.Override(value);
            if (elapsed > duration)
            {
                activateAberration = false;
            }
        }
    }

    public void PlayerEntersPortal()
    {
        if (!activateAberration)
        {
            activateAberration = true;
            startTime = Time.time;
        }
    }
}
