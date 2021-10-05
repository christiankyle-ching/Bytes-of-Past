using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMessage : MonoBehaviour
{
    public GameObject Message;

    public void hideMsg()
    {
        Message.SetActive(false);
    }
}
