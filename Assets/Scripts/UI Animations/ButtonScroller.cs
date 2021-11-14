using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ButtonScroller : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button btnLeft;
    public Button btnRight;

    [Header("Scroll Settings")]
    [Range(0f, 100f)] public float scrollSensitivity;

    ScrollRect scroller;


    private void Start()
    {
        scroller = GetComponent<ScrollRect>();

        btnLeft.onClick.AddListener(() => ScrollContent(-1));
        btnRight.onClick.AddListener(() => ScrollContent(1));
    }

    // positive for right, negative for left
    private void ScrollContent(float direction)
    {
        float realDirection = Mathf.Sign(direction) * -1;
        // Flip the sign again, because it's actually the other way around

        scroller.velocity = new Vector2(realDirection * scrollSensitivity * 100f, 0);
    }

    public void OnScrollerPositionChanged(Vector2 position)
    {
        btnLeft.interactable = scroller.horizontalNormalizedPosition > 0.1f;
        btnRight.interactable = scroller.horizontalNormalizedPosition < 0.9f;
    }
}
