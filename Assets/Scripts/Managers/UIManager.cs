using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;

        }
    }

    [SerializeField] private Image inputHintBackground;
    [SerializeField] private TextMeshProUGUI inputHintText;

    public void DisplayInputHint()
    {
        inputHintBackground.enabled = true;
        inputHintText.enabled = true;
    }

    public void HideInputHint()
    {
        inputHintBackground.enabled = false;
        inputHintText.enabled = false;
    }
}
