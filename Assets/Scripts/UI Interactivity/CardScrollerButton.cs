using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(BoxCollider2D))]
public class CardScrollerButton : MonoBehaviour
{
    [Range(-1, 1)] public int direction = 1;
    public CardScroller cardScroller;

    Button button;
    GameObject _card;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => cardScroller.ScrollContent(direction));
    }

    public void OnHover()
    {
        cardScroller.ScrollContent(direction, true);
    }
}
