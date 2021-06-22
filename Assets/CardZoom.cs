using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour
{
    public float holdTime = 0.5f;

    float lastHoldTime = 0f;

    bool isHolding = false;

    Vector3 originalPos;

    void Start()
    {
        originalPos = gameObject.transform.position;
    }

    void Update()
    {
        if (isHolding)
        {
            lastHoldTime += Time.deltaTime;

            if (lastHoldTime >= holdTime)
            {
                ZoomCard();
            }
        }
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        originalPos = gameObject.transform.position;
        isHolding = true;
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        isHolding = false;
        lastHoldTime = 0f;

        UnzoomCard();
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        isHolding = false;
        lastHoldTime = 0f;

        UnzoomCard();
    }

    void ZoomCard()
    {
        gameObject.transform.localScale = new Vector3(1.5f, 1.5f);
        gameObject.transform.position = new Vector3(0f, 0f);
    }

    void UnzoomCard()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f);
        gameObject.transform.position = originalPos;
    }
}
