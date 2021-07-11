using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomLoader : MonoBehaviour
{
    private bool isLoaded;
    private bool shouldLoad;

    void Start()
    {
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == gameObject.name)
                {
                    isLoaded = true;
                }
            }
        }
    }

    void Update()
    {
        TriggerCheck();
    }

    void LoadScene()
    {
        if (!isLoaded)
        {
            isLoaded = true;
            StartCoroutine(AsyncLoadingAndCheckPortals());
        }
    }

    private IEnumerator AsyncLoadingAndCheckPortals()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        MainCamera.Instance.SearchAllPortals();
    }

    void UnLoadScene()
    {
        if (isLoaded)
        {
            isLoaded = false;
            StartCoroutine(AsyncUnLoadingAndCheckPortals());
        }
    }

    private IEnumerator AsyncUnLoadingAndCheckPortals()
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(gameObject.name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        MainCamera.Instance.SearchAllPortals();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }

    void TriggerCheck()
    {
        if (shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }
}

