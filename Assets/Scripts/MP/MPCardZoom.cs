using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MPCardZoom : NetworkBehaviour
{
    public GameObject zoomCardPrefab;
    public GameObject zoomCard;
    private GameObject canvas;

    private void Awake()
    {
        canvas = GameObject.Find("GameCanvas");
    }

    public void OnPointerEnter()
    {
        zoomCard = Instantiate(zoomCardPrefab);

        zoomCard.transform.SetParent(canvas.transform, false);
        zoomCard.transform.localPosition = new Vector2(0f, 0f);
        zoomCard.transform.localScale = new Vector2(2f, 2f);
    }

    public void OnPointerExit()
    {
        Destroy(zoomCard);
    }
}
