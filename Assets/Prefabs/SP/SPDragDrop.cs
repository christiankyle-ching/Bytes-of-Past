using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPDragDrop : MonoBehaviour
{
    private SPGameController gameController;
    public GameObject placeholderPrefab;
    public GameObject canvas;

    public bool isDragging = false;
    private bool isDraggable = true;
    private Transform startParent;
    private Vector2 startPos;

    private GameObject placeholder;

    private void Start()
    {
        canvas = GameObject.Find("GameCanvas");
        gameController = GameObject.Find("SPGameController").GetComponent<SPGameController>();
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        if (collider != null)
        {
            try
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("DropZones"))
                {
                    placeholder.transform.SetParent(collider.transform);
                }
                else if (collider.tag == "Card")
                {
                    placeholder.transform.SetSiblingIndex(collider.transform.GetSiblingIndex());
                }
            }
            catch (MissingReferenceException)
            {
                // FIXME: dont know what the heck this is all about but it works
            }
            catch (NullReferenceException)
            {
                // FIXME: dont know what the heck this is all about but it works
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isDragging) return;

        Collider2D collider = collision.collider;

        if (collider != null)
        {
            try
            {
                collider.gameObject.GetComponent<CardScrollerButton>().OnHover();
            }
            catch { }
        }
    }

    public void OnBeginDrag()
    {
        if (!isDraggable) return;

        Debug.Log("BeginDrag");

        isDragging = true;

        startParent = transform.parent;
        startPos = transform.position;

        CreatePlaceholder();

        transform.SetParent(canvas.transform, true);
    }

    public void OnEndDrag()
    {
        if (!isDraggable) return;

        isDragging = false;

        if (placeholder.transform.parent.tag == "Timeline")
        {
            // TODO: HandleDropInTimeline
            gameController.HandleDropInTimeline(this.gameObject, placeholder.transform.GetSiblingIndex());

            RemovePlaceholder();
        }
        else
        {
            transform.SetParent(startParent, false);
            transform.position = startPos;

            ReplacePlaceholder();
        }

        startParent = null;

        SoundManager.Instance.PlayDrawSFX();
    }

    private void CreatePlaceholder()
    {
        placeholder = Instantiate(placeholderPrefab, transform.parent, false);
        placeholder.name = "PLACEHOLDER";

        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    public void ReplacePlaceholder()
    {
        int placeholderIndex = placeholder.transform.GetSiblingIndex();

        transform.SetParent(placeholder.transform.parent);
        transform.SetSiblingIndex(placeholderIndex);

        Destroy(placeholder);
    }

    private void RemovePlaceholder()
    {
        Destroy(placeholder);
    }

    public void OnDiscard()
    {
        GetComponent<Animator>().SetTrigger("Wrong");
        Destroy(this.gameObject, 1);
    }

    public void OnPlaceCorrect()
    {
        DisableDrag();
    }

    public void EnableDrag()
    {
        isDraggable = true;
    }

    public void DisableDrag()
    {
        isDraggable = false;
        isDragging = false;
    }

    public void CancelDrag()
    {
        isDragging = false;
        GetComponent<SPCardZoom>().UnzoomCard();
        ResetToOriginalPos();
        RemovePlaceholder();
    }

    public void ResetToOriginalPos()
    {
        try
        {
            if (startParent != null)
            {
                transform.SetParent(startParent, false);
                transform.position = startPos;
            }
        }
        catch { }
    }
}
