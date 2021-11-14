using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(TextMeshProUGUI))]
public class RoundText : MonoBehaviour
{
    public void SetText(string newText)
    {
        string currentText = GetComponent<TextMeshProUGUI>().text;
        if (newText != currentText)
        {
            GetComponent<TextMeshProUGUI>().text = newText;
            GetComponent<Animator>().SetTrigger("Change");
        }
    }
}
