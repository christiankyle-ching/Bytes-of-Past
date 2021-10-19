using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MPCardZoom : NetworkBehaviour
{
    public float holdTime = 0.5f;
    float lastHoldTime = 0f;
    bool isHolding = false;
    bool isZoomed = false;

    public GameObject zoomCardPrefab;
    public GameObject zoomCard;
    private Transform canvas;

    private void Awake()
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

    public void OnPointerDown()
    {
        isHolding = true;
    }

    public void OnPointerUp()
    {
        isHolding = false;
        lastHoldTime = 0f;

        UnzoomCard();
    }

    public void OnBeginDrag()
    {
        isHolding = false;
        lastHoldTime = 0f;

        UnzoomCard();
    }

    void ZoomCard()
    {
        isZoomed = true;

        zoomCard = Instantiate(zoomCardPrefab);
        zoomCard.transform.SetParent(canvas, false);
        zoomCard.transform.localPosition = new Vector2(0f, 0f);
        zoomCard.transform.localScale = new Vector2(2f, 2f);

        zoomCard.GetComponent<MPCardInfo>().InitCardData(GetComponent<MPCardInfo>().cardData);

        //zoomCard.GetComponent<Animator>().SetTrigger("OnPreview");
    }

    void UnzoomCard()
    {
        isZoomed = false;
        if (zoomCard != null) Destroy(zoomCard.gameObject);
    }
}
