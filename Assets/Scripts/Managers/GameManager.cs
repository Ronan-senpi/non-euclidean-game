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
        DontDestroyOnLoad(gameObject);
        postprocess = FindObjectOfType<Volume>();
        if(postprocess)
            postprocess.profile.TryGet(out aberration);

    }

    private void Update()
    {
        if (activateAberration && aberration)
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

    public void PlayerEntersPortal()
    {
        if (!activateAberration)
        {
            activateAberration = true;
            startTime = Time.time;
        }
    }

}
