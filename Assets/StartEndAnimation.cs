using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEndAnimation : Activable
{
    protected override void ActionOnUse(bool action = false)
    {
        StartCoroutine(LaunchEndAnimation());
    }

    private IEnumerator  LaunchEndAnimation()
    {
        GameManager.Instance.FadeIn();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Generic");
    }
}
