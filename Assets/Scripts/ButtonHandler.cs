using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    Vector2 normalScale = new Vector2(1f, 1f);

    Vector2 highlightScale = new Vector2(1.05f, 1.05f);

    // Assign Buttons in Unity Inspector
    public GameObject[] buttons;

    public void OnPointerEnter(BaseEventData data)
    {
        PointerEventData eventData = data as PointerEventData;
        foreach (var button in buttons)
        {
            /* 
            FIXME: pointerCurrentRaycast.gameObject points to Text inside buttons on the first three buttons
            Temporary Solution: Also check for the parent
            */
            if (
                button == eventData.pointerCurrentRaycast.gameObject ||
                button ==
                eventData
                    .pointerCurrentRaycast
                    .gameObject
                    .transform
                    .parent
                    .gameObject
            )
            {
                button.transform.localScale = highlightScale;
            }
            else
            {
                button.transform.localScale = normalScale;
            }
        }
    }
}
