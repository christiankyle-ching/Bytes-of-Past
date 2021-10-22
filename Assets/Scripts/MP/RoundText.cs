using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(TextMeshProUGUI))]
public class RoundText : MonoBehaviour
{
    public void SetText(string newText)
    {
        GetComponent<TextMeshProUGUI>().text = newText;
        GetComponent<Animator>().SetTrigger("OnChange");
    }
}
