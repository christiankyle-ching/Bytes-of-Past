using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;

public enum CARDACTION
{
    Played, Dealt
}

public class PlayerManager : NetworkBehaviour
{
    private int startingCardsCount = 5;

    public GameObject cardPrefab;

    private GameObject playerArea;
    private GameObject enemy0, enemy1, enemy2;
    private GameObject dropzone;

    private List<MPCardData> cardInfos = new List<MPCardData>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerArea");
        dropzone = GameObject.FindGameObjectWithTag("Timeline");

        Transform enemyAreas = GameObject.Find("EnemyAreas").transform;
        enemy0 = enemyAreas.GetChild(0).gameObject;
        enemy1 = enemyAreas.GetChild(1).gameObject;
        enemy2 = enemyAreas.GetChild(2).gameObject;

        LoadCards();
    }

    private void LoadCards()
    {
        List<MPCardData> loadedCards = ParseCSVToCards(MPGameManager.Instance._topic);
        foreach (MPCardData data in loadedCards)
        {
            this.cardInfos.Add(data);
        }
    }

    private List<MPCardData> ParseCSVToCards(TOPIC topic)
    {
        List<MPCardData> cards = new List<MPCardData>();

        TextAsset rawData = null;

        switch (topic)
        {
            case TOPIC.Computer:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Computer");
                break;
            case TOPIC.Networking:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Networking");
                break;
            case TOPIC.Software:
                rawData =
                    Resources
                        .Load
                        <TextAsset
                        >("Cards/Cards - Networking");
                break;
        }

        if (rawData != null)
        {
            List<String> lines = rawData.text.Split('\n').ToList(); // split into lines

            // ignore header
            lines = lines.Skip(3).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] cells = lines[i].Split('\t');

                cards
                    .Add(new MPCardData(cells[0], Int32.Parse(cells[2]), cells[3], cells[4], cells[5]));
            }
        }

        return cards;
    }

    [Command]
    public void CmdDealCards()
    {
        MPGameManager.Instance.AddReadyPlayerCount();

        for (int i = 0; i < startingCardsCount; i++)
        {
            int infoIndex = MPGameManager.Instance.PopCard();

            if (infoIndex < 0) break; // if deck has no cards

            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt);
        }

        RpcUpdateTurn();
    }

    public void PlayCard(GameObject card, int infoIndex, int pos)
    {
        CmdPlayCard(card, infoIndex, pos);
    }

    [Command]
    private void CmdPlayCard(GameObject card, int infoIndex, int pos)
    {
        bool isDropValid = MPGameManager.Instance.OnPlayCard(infoIndex, pos);

        if (isDropValid)
        {
            // Show card only if card drop is right
            RpcShowCard(card, infoIndex, pos, CARDACTION.Played);
        }
        else
        {
            card.GetComponent<MPDragDrop>().OnDiscard();
            StartCoroutine(DestroyObjectWithDelay(card));
        }

        RpcUpdateTurn();
    }

    [ClientRpc]
    public void RpcUpdateTurn()
    {
        if (!MPGameManager.Instance.gameStarted) return;

        bool yourTurn = MPGameManager.Instance.currentPlayer == netIdentity;

        // Enable or disable cards only working on host
        if (yourTurn)
        {
            Debug.Log("YOUR TURN!");
            foreach (MPDragDrop dragDrop in playerArea.GetComponentsInChildren<MPDragDrop>())
            {
                dragDrop.EnableDrag();
            }
        }
        else
        {
            Debug.Log("NOT YOUR TURN!");
            foreach (MPDragDrop dragDrop in playerArea.GetComponentsInChildren<MPDragDrop>())
            {
                dragDrop.DisableDrag();
            }
        }
    }

    [ClientRpc]
    public void RpcShowCard(GameObject card, int infoIndex, int pos, CARDACTION type)
    {
        card.GetComponent<MPCardInfo>().InitCardData(cardInfos[infoIndex]);
        card.GetComponent<MPCardInfo>().infoIndex = infoIndex;
        card.GetComponent<MPDragDrop>().DisableDrag();

        if (type == CARDACTION.Dealt)
        {
            if (hasAuthority)
            {
                card.transform.SetParent(playerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(enemy0.transform, false);
            }
        }
        else if (type == CARDACTION.Played)
        {
            card.transform.SetParent(dropzone.transform, false);
            card.transform.SetSiblingIndex(pos);
            card.GetComponent<Animator>().SetTrigger("Correct");
        }
    }

    IEnumerator DestroyObjectWithDelay(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(obj);
    }

}
