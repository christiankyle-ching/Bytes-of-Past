using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEllipsisAnimation : MonoBehaviour
{
    public float interval = 1f;
    private float timeLeft = 0;
    private string baseText = "";
    private TextMeshProUGUI textGO;
    private string loadingText = "...";
    private int currentLength = 0;

    private void Start()
    {
        textGO = GetComponent<TextMeshProUGUI>();
        baseText = textGO.text;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        currentLength = (currentLength + 1 <= loadingText.Length) ? currentLength + 1 : 0;
        textGO.text = baseText + loadingText.Substring(0, currentLength);
        timeLeft = interval;
    }
}
