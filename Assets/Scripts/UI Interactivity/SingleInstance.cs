using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstance : MonoBehaviour
{
    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(this.tag).Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
