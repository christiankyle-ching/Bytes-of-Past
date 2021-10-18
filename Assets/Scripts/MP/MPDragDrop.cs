using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class MPDragDrop : NetworkBehaviour
{
    private GameObject canvas; // GameCanvas
    public PlayerManager playerManager;
    
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private bool isDraggable = false;
    private GameObject dropzone; // last dropzone the card entered
    private Transform startParent;
    private Vector2 startPos;

    private void Start()
    {
        canvas = GameObject.Find("GameCanvas");
        isDraggable = hasAuthority;
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
        if (!isDraggable) return;

        isDragging = true;

        startParent = transform.parent;
        startPos = transform.position;
    }

    public void OnEndDrag()
    {
        if (!isDraggable) return;

        isDragging = false;

        if (isOverDropZone)
        {
            transform.SetParent(dropzone.transform, false);
            isDraggable = false;

            NetworkIdentity ni = NetworkClient.connection.identity;
            playerManager = ni.GetComponent<PlayerManager>();
            playerManager.PlayCard(gameObject);
        } else
        {
            transform.SetParent(startParent, false);
            transform.position = startPos;
        }
    }
}
