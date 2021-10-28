using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public enum CARDACTION
{
    Played, Dealt
}

public class PlayerManager : NetworkBehaviour
{
    private bool isMyTurn = false;
    private int startingCardsCount = 5; // TODO: Set in Prod

    public GameObject cardPrefab;

    private GameObject playerArea;
    private GameObject enemyAreas;
    private MPGameMessage messenger;
    private GameObject gameStateText;
    private GameObject activeSpecialAction;
    private GameObject deck;
    private GameObject dropzone;
    private MPTimer timer;
    private MPCanvasHUD menuCanvas;
    private MPQuestionManager questionManager;

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
        activeSpecialAction = GameObject.Find("ActiveSpecialAction");
        deck = GameObject.Find("Deck");
        timer = GameObject.Find("MPTimer").GetComponent<MPTimer>();
        dropzone = GameObject.FindGameObjectWithTag("Timeline");
        menuCanvas = GameObject.Find("MenuCanvas").GetComponent<MPCanvasHUD>();
        messenger = GameObject.Find("MESSENGER").GetComponent<MPGameMessage>();
        questionManager = GameObject.Find("RoundQuestion").GetComponent<MPQuestionManager>();

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
    public void CmdReady(string playerName)
    {
        for (int i = 0; i < startingCardsCount; i++)
        {
            int infoIndex = MPGameManager.Instance.PopCard(netIdentity);
            SPECIALACTION randSpecial = MPGameManager.Instance.GetRandomSpecialAction();

            if (infoIndex < 0) break; // if deck has no cards

            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt, randSpecial);
        }

        NetworkIdentity ni = connectionToClient.identity;
        MPGameManager.Instance.SetPlayerName(ni, playerName);
        MPGameManager.Instance.ReadyPlayer(ni);
    }

    [Command(requiresAuthority = false)]
    public void CmdGetAnotherCard()
    {
        int infoIndex = MPGameManager.Instance.PopCard(connectionToClient.identity);
        SPECIALACTION randSpecial = MPGameManager.Instance.GetRandomSpecialAction();

        if (infoIndex < 0) return; // if deck has no cards

        GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt, randSpecial);
    }

    public void PlayCard(GameObject card, int infoIndex, int pos, bool hasDrop = true)
    {
        if (hasAuthority)
        {
            if (hasDrop)
            {
                CmdPlayCard(card, infoIndex, pos, hasDrop, card.GetComponent<MPCardInfo>().cardData.SpecialAction);
            }
            else
            {
                CmdPlayCard(card, infoIndex, pos, hasDrop, SPECIALACTION.None);
            }

            isMyTurn = false;
        }
    }

    [Command]
    private void CmdPlayCard(GameObject card, int infoIndex, int pos, bool hasDrop, SPECIALACTION special)
    {
        bool isDropValid = MPGameManager.Instance.OnPlayCard(infoIndex, pos, netIdentity, hasDrop, special);

        if (hasDrop)
        {
            if (isDropValid)
            {
                // Show card only if card drop is right
                RpcShowCard(card, infoIndex, pos, CARDACTION.Played, special);
            }
            else
            {
                // Discard the dropped card
                NetworkConnection conn = card.GetComponent<NetworkIdentity>().connectionToClient;
                TargetDiscard(conn, card);
            }
        }

        timer.StopTimer();
    }

    [TargetRpc]
    private void TargetDiscard(NetworkConnection conn, GameObject card)
    {
        card.GetComponent<MPDragDrop>().OnDiscard(); // Animator.SetTrigger()
        StartCoroutine(DestroyObjectWithDelay(card));
    }

    #region ------------------------------ UPDATE GUI FUNCTIONS ------------------------------

    [ClientRpc]
    public void RpcUpdateUI(NetworkIdentity currentPlayer,
        int currentPlayerIndex,
        NetworkIdentity[] players,
        string[] playerNames,
        int deckCount,
        bool gameFinished,
        string gameMsg,
        string[] winnerNames,
        NetworkIdentity[] winnerIdens,
        string question,
        string[] choices)
    {
        if (gameFinished)
        {
            menuCanvas.ShowEndGameMenu(winnerNames, winnerIdens);
        }
        else
        {
            UpdateTurn(currentPlayer, gameFinished, gameMsg, question, choices);
            UpdateOpponentCards(currentPlayer, currentPlayerIndex, players, /*playerHands,*/ playerNames);
            UpdateDeckCount(deckCount);
        }

    }

    public void UpdateTurn(NetworkIdentity identity, bool gameFinished, string gameMsg, string question, string[] choices)
    {
        isMyTurn = identity.isLocalPlayer && MPGameManager.Instance.gameStarted;

        Debug.Log($"ILP: {isMyTurn}");

        // Disable IP Address when game already started
        if (MPGameManager.Instance.gameStarted)
        {
            try
            {
                GameObject.Find("IP-ADDRESS").SetActive(false);
            }
            catch { }
        }

        if (question != null && choices != null)
        {
            questionManager.ShowQuestion(question, choices);
            timer.StartQuizTimer();
        }
        else
        {
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
    }

    [ClientRpc]
    public void RpcShowDoubleDraw(bool doubleDrawActive)
    {
        if (doubleDrawActive)
        {
            activeSpecialAction.GetComponentInChildren<TextMeshProUGUI>().text = "Next Player to make mistake gets double card!";
            activeSpecialAction.GetComponentInChildren<Image>().sprite = MPCardInfo.GetSpecialActionSprite(SPECIALACTION.DoubleDraw);
            activeSpecialAction.GetComponentInChildren<Image>().color = Color.white;
        }
        else
        {
            activeSpecialAction.GetComponentInChildren<TextMeshProUGUI>().text = "";
            activeSpecialAction.GetComponentInChildren<Image>().sprite = null;
            activeSpecialAction.GetComponentInChildren<Image>().color = Color.clear;
        }
    }

    public void UpdateOpponentCards(
        NetworkIdentity currentPlayer,
        int currentPlayerIndex,
        NetworkIdentity[] players,
        string[] playerNames)
    {
        List<int> tmpOpponentIndex = new List<int>();
        int opponentCurrentIndex = -1;

        // Get Opponent Indices only
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer) continue;

            if (currentPlayerIndex == i) opponentCurrentIndex = tmpOpponentIndex.Count;

            tmpOpponentIndex.Add(i);
        }

        Debug.Log("Players: " + String.Join(",", players.Select(iden => iden.netId)));
        Debug.Log("Opponent Indexes: " + String.Join(",", tmpOpponentIndex));

        foreach (KeyValuePair<uint, List<int>> kvp in MPGameManager.Instance.playerHands)
        {
            Debug.Log($"Player #{kvp.Key} Cards: {String.Join(",", kvp.Value)}");
        }

        //Update UI
        for (int i = 0; i < enemyAreas.transform.childCount; i++)
        {
            TextMeshProUGUI cardCount = enemyAreas.transform.GetChild(i).Find("CardsRemaining").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerNameGO = enemyAreas.transform.GetChild(i).Find("PlayerName").GetComponent<TextMeshProUGUI>();

            if (i < tmpOpponentIndex.Count)
            {
                int playerIndex = tmpOpponentIndex[i];
                uint playerId = players[playerIndex].netId;

                string playerName = playerNames[playerIndex];
                int handCount = MPGameManager.Instance.GetPlayerHand(playerId).Count;

                string cardMsg = $"{handCount} Cards";

                Debug.Log($"Player #{playerId} has {handCount}");

                playerNameGO.gameObject.GetComponent<TextEllipsisAnimation>().enabled = false;

                if (MPGameManager.Instance.gameStarted)
                {
                    // Highlight current player if game started
                    if (opponentCurrentIndex == i) cardMsg += " (Playing)";

                    // Highlight player when only 1 card remaining
                    if (handCount <= 1)
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

                playerNameGO.text = playerName;
                cardCount.text = cardMsg;
            }
            else
            {
                if (MPGameManager.Instance.gameStarted)
                {
                    playerNameGO.gameObject.GetComponent<TextEllipsisAnimation>().enabled = false;
                    playerNameGO.text = "";
                    cardCount.text = "";
                }
                else
                {
                    playerNameGO.text = "Waiting For Players";
                    playerNameGO.gameObject.GetComponent<TextEllipsisAnimation>().enabled = true;
                }
            }
        }
    }

    public void UpdateDeckCount(int deckCount)
    {
        deck.GetComponentInChildren<TextMeshProUGUI>().text = $"{deckCount} Cards Remaining";
    }

    [ClientRpc]
    public void RpcShowCard(GameObject card, int infoIndex, int pos, CARDACTION type, SPECIALACTION special)
    {
        card.GetComponent<MPCardInfo>().InitCardData(cardInfos[infoIndex], special);
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
            card.GetComponent<MPCardInfo>().RevealCard();
        }
    }

    [ClientRpc]
    public void RpcGameInterrupted(string playerName, bool isHost)
    {
        menuCanvas.ShowInterruptedGame(playerName, isHost);
    }

    [TargetRpc]
    public void TargetPeekCard(NetworkConnection conn)
    {
        Debug.Log("Activate Peek");

        int cardCount = playerArea.transform.childCount;

        if (cardCount > 0)
        {
            int randIndex = UnityEngine.Random.Range(0, cardCount);
            playerArea.transform.GetChild(randIndex).GetComponent<MPCardInfo>().RevealCard();
        }
    }

    [TargetRpc]
    public void TargetDiscardRandomHand(NetworkConnection conn, int infoIndex)
    {
        string cardIdToRemove = this.cardInfos[infoIndex].ID;

        foreach (GameObject card in playerArea.transform)
        {

            if (card.GetComponent<MPCardInfo>().cardData.ID == cardIdToRemove)
            {
                card.GetComponent<MPDragDrop>().OnDiscard();
                StartCoroutine(DestroyObjectWithDelay(card));
                break;
            }
        }
    }

    [ClientRpc]
    public void RpcShowGameMessage(string message, MPGameMessageType type)
    {
        messenger.ShowMessage(message, type);
    }

    [TargetRpc]
    public void TargetShowQuizResult(NetworkConnection conn, string message, MPGameMessageType type)
    {
        messenger.ShowMessage(message, type);
        questionManager.SetVisibility(false);
    }
    #endregion

    #region ------------------------------ UTILS ------------------------------

    IEnumerator DestroyObjectWithDelay(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(obj);
    }

    #endregion

    #region ------------------------------ QUESTION MANAGER ------------------------------

    public void AnswerQuiz(string answer)
    {
        CmdAnswerQuiz(answer);
    }

    [Command]
    public void CmdAnswerQuiz(string answer)
    {
        MPGameManager.Instance.OnAnswerQuiz(netIdentity, answer);
    }

    #endregion
}
