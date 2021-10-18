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

    private Stack<CardData> cards = new Stack<CardData>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerArea");
        dropzone = GameObject.FindGameObjectWithTag("Timeline");

        Transform enemyAreas = GameObject.Find("EnemyAreas").transform;
        enemy0 = enemyAreas.GetChild(0).gameObject;
        enemy1 = enemyAreas.GetChild(1).gameObject;
        enemy2 = enemyAreas.GetChild(2).gameObject;
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        LoadCards();
    }

    [Server]
    private void LoadCards()
    {
        List<CardData> loadedCards = ParseCSVToCards(TOPIC.Computer);
        foreach (CardData data in loadedCards)
        {
            this.cards.Push(data);
        }

        ShuffleCards();

        Debug.Log($"Deck: {this.cards.Count}");
    }

    [Command]
    public void CmdDealCards()
    {
        for (int i = 0; i < startingCardsCount; i++)
        {
            CardData data = cards.Pop(); // TODO: Use this card data
            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);

            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, CARDACTION.Dealt);
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    private void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, CARDACTION.Played);
    }

    [ClientRpc]
    public void RpcShowCard(GameObject card, CARDACTION type)
    {
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
        }
    }

    [Server]
    private List<CardData> ParseCSVToCards(TOPIC topic)
    {
        List<CardData> cards = new List<CardData>();

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
                    .Add(new CardData(cells[0], Int32.Parse(cells[2]), cells[3], cells[4], cells[5]));
            }
        }

        return cards;
    }

    [Server]
    private void ShuffleCards()
    {
        Stack<CardData> shuffledCards = new Stack<CardData>();

        foreach (CardData cardData in this.cards.OrderBy(x => UnityEngine.Random.Range(0f, 1f)))
        {
            shuffledCards.Push(cardData);
        }

        this.cards = shuffledCards;
    }
}
