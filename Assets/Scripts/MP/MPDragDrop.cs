using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MPDragDrop : MonoBehaviour
{
    private GameObject canvas; // GameCanvas
    private GameObject dropzone; // last dropzone the card entered

    private bool isDragging = false;
    private bool isOverDropZone = false;
    private Transform startParent;
    private Vector2 startPos;

    private void Start()
    {
        canvas = GameObject.Find("GameCanvas");
        dropzone = transform.parent.gameObject;
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.position = Input.mousePosition;
            transform.SetParent(canvas.transform, true);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropzone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropzone = null;
    }

    public void OnBeginDrag()
    {
        isDragging = true;

        startParent = transform.parent;
        startPos = transform.position;
    }

    public void OnEndDrag()
    {
        isDragging = false;

        if (isOverDropZone)
        {
            transform.SetParent(dropzone.transform, false);
        } else
        {
            transform.SetParent(startParent, false);
            transform.position = startPos;
        }
    }
}
