using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using TMPro;

public enum CARDACTION
{
    Played, Dealt
}

public class PlayerManager : NetworkBehaviour
{
    private bool isMyTurn = false;

    private int startingCardsCount = 5;

    public GameObject cardPrefab;

    private GameObject playerArea;
    private GameObject enemyAreas;
    private GameObject dropzone;

    private List<MPCardData> cardInfos = new List<MPCardData>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerArea");
        enemyAreas = GameObject.Find("EnemyAreas");
        dropzone = GameObject.FindGameObjectWithTag("Timeline");

        LoadCards();
    }

    #region ------------------------------ LOCAL LOAD CARDS ------------------------------

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

    #endregion

    [Command]
    public void CmdDealCards()
    {
        MPGameManager.Instance.AddReadyPlayerCount();

        for (int i = 0; i < startingCardsCount; i++)
        {
            int infoIndex = MPGameManager.Instance.PopCard(netIdentity);

            if (infoIndex < 0) break; // if deck has no cards

            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt);
        }

        StartCoroutine(nameof(UpdateGUI));

        //RpcUpdateTurn();
        //RpcUpdateOpponentCards();
        //RpcUpdateDeckCount();
    }

    public void PlayCard(GameObject card, int infoIndex, int pos)
    {
        CmdPlayCard(card, infoIndex, pos);
    }

    [Command]
    private void CmdPlayCard(GameObject card, int infoIndex, int pos)
    {
        bool isDropValid = MPGameManager.Instance.OnPlayCard(infoIndex, pos, netIdentity);

        if (isDropValid)
        {
            // Show card only if card drop is right
            RpcShowCard(card, infoIndex, pos, CARDACTION.Played);
        }
        else
        {
            card.GetComponent<MPDragDrop>().OnDiscard();
            StartCoroutine(DestroyObjectWithDelay(card));
            MPGameManager.Instance.PopCard(netIdentity); // Get another card
        }

        //StartCoroutine(nameof(UpdateGUI));

        //RpcUpdateTurn();
        //RpcUpdateOpponentCards();
        //RpcUpdateDeckCount();
    }

    #region ------------------------------ UPDATE GUI FUNCTIONS ------------------------------

    public IEnumerable UpdateGUI()
    {
        yield return new WaitForSeconds(1f);

        //RpcUpdateTurn();
        //RpcUpdateOpponentCards();
        //RpcUpdateDeckCount();
    }

    [ClientRpc]
    public void RpcUpdateUI(NetworkIdentity currentPlayer, int currentPlayerIndex, NetworkIdentity[] players, int[] playerHands, int deckCount)
    {
        RpcUpdateTurn(currentPlayer);
        RpcUpdateOpponentCards(currentPlayer, currentPlayerIndex, players, playerHands);
        RpcUpdateDeckCount(deckCount);
    }

    public void RpcUpdateTurn(NetworkIdentity identity)
    {

        isMyTurn = identity.isLocalPlayer;

        Debug.Log($"ILP: {isMyTurn}");

        // Enable or disable cards
        if (isMyTurn)
        {
            //Debug.Log("YOUR TURN!");
            foreach (MPDragDrop dragDrop in playerArea.GetComponentsInChildren<MPDragDrop>())
            {
                dragDrop.EnableDrag();
            }
        }
        else
        {
            //Debug.Log("NOT YOUR TURN!");
            foreach (MPDragDrop dragDrop in playerArea.GetComponentsInChildren<MPDragDrop>())
            {
                dragDrop.DisableDrag();
            }
        }
    }

    public void RpcUpdateOpponentCards(NetworkIdentity currentPlayer, int currentPlayerIndex, NetworkIdentity[] players, int[] playerHands)
    {
        List<int> tmpOpponentHands = new List<int>();

        int opponentCurrentIndex = -1;

        Debug.Log("ILP Cards: " + currentPlayer.isLocalPlayer);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer) continue;
            if (currentPlayerIndex == i) opponentCurrentIndex = tmpOpponentHands.Count;
            tmpOpponentHands.Add(playerHands[i]);
        }

        for (int i = 0; i < tmpOpponentHands.Count; i++)
        {
            Transform cardCount = enemyAreas.transform.GetChild(i).Find("CardsRemaining");
            string message = tmpOpponentHands[i].ToString() + " Cards Remaining";
            if (opponentCurrentIndex == i) message += " (Playing)";

            cardCount.GetComponent<TextMeshProUGUI>().text = message;
        }

        Debug.Log("Player Hands: " + String.Join(",", playerHands));
        Debug.Log("Opponent Hands: " + String.Join(",", tmpOpponentHands));
    }

    public void RpcUpdateDeckCount(int deckCount)
    {
        // TODO: Deck Count
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
            //else
            //{
            //    card.transform.SetParent(enemy0.transform, false);
            //}
        }
        else if (type == CARDACTION.Played)
        {
            card.transform.SetParent(dropzone.transform, false);
            card.transform.SetSiblingIndex(pos);
            card.GetComponent<Animator>().SetTrigger("Correct");
        }
    }

    #endregion

    #region ------------------------------ UTILS ------------------------------

    IEnumerator DestroyObjectWithDelay(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(obj);
    }

    #endregion
}
