using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    void LaunchGameAfterAnimaton()
    {
        GameManager.Instance.LoadScene(1);
    }
}
