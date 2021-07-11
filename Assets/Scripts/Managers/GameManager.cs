using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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

    [Header("Menu Parameters")]
    [SerializeField] private Animator launchGameAnimation;

    [Space(10)]
    [Header("Post Process Parameters")]
    [SerializeField] private Volume postprocess;
    [SerializeField] private AnimationCurve aberrationFadeCurve;
    [SerializeField] private float aberrationDuration;
    [SerializeField] private float aberrationIntensity;

    [SerializeField] private AnimationCurve lensFadeCurve;
    [SerializeField] private float lensDuration;
    [SerializeField] private float lensIntensity;

    private float currentValue;
    private float startTime;

    private bool activateAberration;
    private bool activateLens;
    private ChromaticAberration aberration;
    private LensDistortion lens;

    private void Awake()
    {
        postprocess = FindObjectOfType<Volume>();
        if (postprocess)
        {
            postprocess.profile.TryGet(out aberration);
            postprocess.profile.TryGet(out lens);
        }

    }

    private void Update()
    {
        if (activateAberration && aberration)
        {
            float elapsed = Time.time - startTime;
            float value = aberrationIntensity * aberrationFadeCurve.Evaluate(elapsed / aberrationDuration);
            aberration.intensity.Override(value);
            if (elapsed > aberrationDuration)
            {
                activateAberration = false;
            }
        }

        if(activateLens && lens) {
            float elapsed = Time.time - startTime;
            float value = lensIntensity * lensFadeCurve.Evaluate(elapsed / lensDuration);
            lens.intensity.Override(value);
            if (elapsed > lensDuration)
            {
                activateLens = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void LaunchStartAnimation()
    {
        launchGameAnimation.SetTrigger("LaunchGame");
    }

    public void LoadScene(int index)
    {
        StartCoroutine(AsyncLoading(index));
    }

    private IEnumerator AsyncLoading(int id)
    {
        float chrono = 0;

        while (chrono <= 0.25f)
        {
            chrono += Time.deltaTime;
            yield return null;
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ActivateAberration()
    {
        if (!activateAberration)
        {
            activateAberration = true;
            startTime = Time.time;
        }
    }

    public void ActivateLens()
    {
        if (!activateLens)
        {
            activateLens = true;
            startTime = Time.time;
        }
    }

}
