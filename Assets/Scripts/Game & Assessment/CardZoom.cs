using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoom : MonoBehaviour
{
    public float holdTime = 0.5f;

    float lastHoldTime = 0f;

    bool isHolding = false;

    bool isZoomed = false;

    Transform canvas;

    GameObject previewCard;

    void Start()
    {
        canvas = GameObject.Find("GameCanvas").transform;
    }

    void Update()
    {
        if (isHolding && !isZoomed)
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
        isZoomed = true;
        previewCard = Instantiate(this.gameObject, canvas);
        previewCard.transform.position = new Vector3(0f, 0f);

        previewCard.GetComponent<Animator>().SetTrigger("OnPreview");
    }

    void UnzoomCard()
    {
        isZoomed = false;
        if (previewCard != null) Destroy(previewCard.gameObject);
    }
}
