using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using System;
using UnityEngine.UI;

public class MPDragDrop : NetworkBehaviour
{
    public GameObject placeholderPrefab;
    private GameObject canvas; // GameCanvas
    public PlayerManager playerManager;

    private bool isDragging = false;
    private bool isDraggable = false;
    private Transform startParent;
    private Vector2 startPos;

    private GameObject placeholder;

    private void Start()
    {
        canvas = GameObject.Find("GameCanvas");
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

    public void OnBeginDrag()
    {
        if (!isDraggable) return;

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
            NetworkIdentity ni = NetworkClient.connection.identity;
            playerManager = ni.GetComponent<PlayerManager>();
            playerManager.PlayCard(gameObject, GetComponent<MPCardInfo>().infoIndex, placeholder.transform.GetSiblingIndex());

            isDraggable = false;

            RemovePlaceholder();
        }
        else
        {
            transform.SetParent(startParent, false);
            transform.position = startPos;

            ReplacePlaceholder();
        }

        GetComponent<AudioSource>().Play();
    }

    private void CreatePlaceholder()
    {
        placeholder = Instantiate(placeholderPrefab);
        placeholder.name = "PLACEHOLDER";

        placeholder.transform.SetParent(transform.parent);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void ReplacePlaceholder()
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
    }

    public void DisableDrag()
    {
        isDraggable = false;
    }

    public void EnableDrag()
    {
        isDraggable = true;
    }
}
