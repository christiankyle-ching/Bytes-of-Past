using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
