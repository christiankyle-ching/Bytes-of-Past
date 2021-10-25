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

    private int startingCardsCount = 2;

    public GameObject cardPrefab;

    private GameObject playerArea;
    private GameObject enemyAreas;

    private GameObject gameStateText;
    private GameObject deck;
    private GameObject dropzone;
    private MPTimer timer;
    private MPCanvasHUD menuCanvas;

    [Header("Colors")]
    public Color normalTextColor = Color.white;
    public Color dangerTextColor = Color.red;

    private List<CardData> cardInfos = new List<CardData>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerArea");
        enemyAreas = GameObject.Find("EnemyAreas");
        gameStateText = GameObject.Find("RoundText");
        deck = GameObject.Find("Deck");
        timer = GameObject.Find("MPTimer").GetComponent<MPTimer>();
        dropzone = GameObject.FindGameObjectWithTag("Timeline");
        menuCanvas = GameObject.Find("MenuCanvas").GetComponent<MPCanvasHUD>();

        LoadCards();
    }

    #region ------------------------------ LOCAL LOAD CARDS ------------------------------

    private void LoadCards()
    {
        CardData[] loadedCards = ResourceParser.Instance.ParseCSVToCards(MPGameManager.Instance._topic);
        foreach (CardData data in loadedCards)
        {
            this.cardInfos.Add(data);
        }
    }
    #endregion

    [Command]
    public void CmdReady()
    {
        for (int i = 0; i < startingCardsCount; i++)
        {
            int infoIndex = MPGameManager.Instance.PopCard(netIdentity);

            if (infoIndex < 0) break; // if deck has no cards

            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt);
        }

        NetworkIdentity ni = connectionToClient.identity;
        MPGameManager.Instance.SetPlayerName(ni, PlayerPrefs.GetString("Profile_Name", ""));

        MPGameManager.Instance.ReadyPlayer(ni);
    }

    public void PlayCard(GameObject card, int infoIndex, int pos, bool hasDrop = true)
    {
        if (hasAuthority)
        {
            CmdPlayCard(card, infoIndex, pos, hasDrop);
            isMyTurn = false;
        }
    }

    [Command]
    private void CmdPlayCard(GameObject card, int infoIndex, int pos, bool hasDrop)
    {
        bool isDropValid = MPGameManager.Instance.OnPlayCard(infoIndex, pos, netIdentity, hasDrop);

        if (hasDrop)
        {
            if (isDropValid)
            {
                // Show card only if card drop is right
                RpcShowCard(card, infoIndex, pos, CARDACTION.Played);
            }
            else
            {
                // Discard the dropped card
                NetworkConnection conn = card.GetComponent<NetworkIdentity>().connectionToClient;
                TargetDiscard(conn, card);
                StartCoroutine(DestroyObjectWithDelay(card));
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdGetAnotherCard()
    {
        int infoIndex = MPGameManager.Instance.PopCard(connectionToClient.identity);

        if (infoIndex < 0) return; // if deck has no cards

        GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt);
    }

    [TargetRpc]
    private void TargetDiscard(NetworkConnection conn, GameObject card)
    {
        card.GetComponent<MPDragDrop>().OnDiscard(); // Animator.SetTrigger()
    }

    #region ------------------------------ UPDATE GUI FUNCTIONS ------------------------------

    [ClientRpc]
    public void RpcUpdateUI(NetworkIdentity currentPlayer, int currentPlayerIndex, NetworkIdentity[] players, int[] playerHands, string[] playerNames, int deckCount, bool gameFinished, string gameMsg, string[] winnerNames, NetworkIdentity[] winnerIdens)
    {
        if (gameFinished)
        {
            menuCanvas.ShowEndGameMenu(winnerNames, winnerIdens);
        }

        UpdateTurn(currentPlayer, gameFinished, gameMsg);
        UpdateOpponentCards(currentPlayer, currentPlayerIndex, players, playerHands, playerNames);
        UpdateDeckCount(deckCount);
    }

    public void UpdateTurn(NetworkIdentity identity, bool gameFinished, string gameMsg)
    {
        isMyTurn = identity.isLocalPlayer && MPGameManager.Instance.gameStarted;

        Debug.Log($"ILP: {isMyTurn}");

        if (MPGameManager.Instance.gameStarted)
        {
            try
            {
                // Disable IP Address when game already started
                GameObject.Find("IP-ADDRESS").SetActive(false);
            }
            catch { }
        }

        if (isMyTurn && !gameFinished)
        {
            timer.StartTimer();
            foreach (MPDragDrop dragDrop in playerArea.GetComponentsInChildren<MPDragDrop>())
            {
                dragDrop.EnableDrag();
            }
        }
        else
        {
            foreach (MPDragDrop dragDrop in playerArea.GetComponentsInChildren<MPDragDrop>())
            {
                dragDrop.DisableDrag();
            }
        }

        gameStateText.GetComponent<RoundText>().SetText(
            (isMyTurn && MPGameManager.Instance.gameStarted) ?
            $"{gameMsg} (Your Turn)" :
            gameMsg);
    }

    public void UpdateOpponentCards(NetworkIdentity currentPlayer, int currentPlayerIndex, NetworkIdentity[] players, int[] playerHands, string[] playerNames)
    {
        List<string> tmpOpponentNames = new List<string>();
        List<int> tmpOpponentHands = new List<int>();
        int opponentCurrentIndex = -1;

        // Populate Names and HandCounts
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer) continue;

            // .Count because the next index would be the current count before adding the elements
            if (currentPlayerIndex == i) opponentCurrentIndex = tmpOpponentHands.Count;

            tmpOpponentNames.Add(playerNames[i]);
            tmpOpponentHands.Add(playerHands[i]);
        }

        // Update UI
        for (int i = 0; i < enemyAreas.transform.childCount; i++)
        {
            TextMeshProUGUI cardCount = enemyAreas.transform.GetChild(i).Find("CardsRemaining").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerName = enemyAreas.transform.GetChild(i).Find("PlayerName").GetComponent<TextMeshProUGUI>();

            if (i < tmpOpponentHands.Count)
            {
                int handCount = tmpOpponentHands[i];
                string cardMsg = $"{handCount} Cards";

                playerName.gameObject.GetComponent<TextEllipsisAnimation>().enabled = false;

                if (MPGameManager.Instance.gameStarted)
                {
                    // Highlight current player if game started
                    if (opponentCurrentIndex == i) cardMsg += " (Playing)";

                    // Highlight player when no cards remaining
                    if (handCount <= 0)
                    {
                        cardCount.fontStyle = FontStyles.Bold;
                        cardCount.color = dangerTextColor;
                    }
                    else
                    {
                        cardCount.fontStyle = FontStyles.Normal;
                        cardCount.color = normalTextColor;
                    }
                }

                playerName.text = tmpOpponentNames[i];
                cardCount.text = cardMsg;
            }
            else
            {
                if (MPGameManager.Instance.gameStarted)
                {
                    playerName.gameObject.GetComponent<TextEllipsisAnimation>().enabled = false;
                    playerName.text = "";
                    cardCount.text = "";
                }
                else
                {
                    playerName.text = "Waiting For Players";
                    playerName.gameObject.GetComponent<TextEllipsisAnimation>().enabled = true;
                }
            }


        }

        //Debug.Log($"Current Player [{currentPlayerIndex}]");
        //Debug.Log($"My Player? {currentPlayer.isLocalPlayer}");
        //Debug.Log("Player Names: " + String.Join(",", playerNames));
        //Debug.Log("Player Hands: " + String.Join(",", playerHands));
        //Debug.Log("Opponent Hands: " + String.Join(",", tmpOpponentHands));
    }

    public void UpdateDeckCount(int deckCount)
    {
        deck.GetComponentInChildren<TextMeshProUGUI>().text = $"{deckCount} Cards Remaining";
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
        }
        else if (type == CARDACTION.Played)
        {
            card.transform.SetParent(dropzone.transform, false);
            card.transform.SetSiblingIndex(pos);
            card.GetComponent<Animator>().SetTrigger("Correct");
        }
    }

    [ClientRpc]
    public void RpcGameInterrupted(string playerName)
    {
        menuCanvas.ShowInterruptedGame(playerName);
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
