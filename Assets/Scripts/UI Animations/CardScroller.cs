using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CardScroller : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button btnLeft;
    public Button btnRight;

    [Header("Scroll Settings")]
    [Range(0f, 100f)] public float scrollSensitivity = 40f;
    [Range(0f, 100f)] public float hoveringScrollSensitivity = 10f;

    ScrollRect scroller;

    private void Start()
    {
        scroller = GetComponent<ScrollRect>();
    }

    // positive for right, negative for left
    public void ScrollContent(float direction, bool isHovering = false)
    {
        // Disable scrolling when near the edges
        if (direction < 0 && scroller.horizontalNormalizedPosition < 0.1f) return;
        if (direction > 0 && scroller.horizontalNormalizedPosition > 0.9f) return;

        float scrollDirection = Mathf.Sign(direction) * -1;
        // Flip the sign again, because it's actually the other way around

        float sensitivity = isHovering ? hoveringScrollSensitivity : scrollSensitivity;
        scroller.velocity = new Vector2(scrollDirection * sensitivity * 100f, 0);
    }

    public void OnScrollerPositionChanged(Vector2 position)
    {
        btnLeft.interactable = scroller.horizontalNormalizedPosition > 0.1f;
        btnRight.interactable = scroller.horizontalNormalizedPosition < 0.9f;
    }

}
