using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBar;

    public void LaunchGame()
    {
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int id)
    {
        float chrono = 0;

        while (chrono <= 0.25f)
        {
            chrono += Time.deltaTime;
            yield return null;
        }


        loadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);
        loadingBar.fillAmount = 0f;

        while (!asyncLoad.isDone)
        {
            loadingBar.fillAmount = asyncLoad.progress;
            yield return null;
        }

        loadingScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
