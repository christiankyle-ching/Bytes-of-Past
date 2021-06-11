using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{

    public float scrollSpeed;
    private Vector3 StartingPos;
    // Start is called before the first frame update
    void Start()
    {

        StartingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(translation: Vector3.right * scrollSpeed * Time.deltaTime);
    }
}
