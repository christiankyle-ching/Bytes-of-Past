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
    private TOPIC _topic = TOPIC.Computer;
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

    //private List<CardData> cardInfos = new List<CardData>();

    private List<CardData> computerCards = new List<CardData>();
    private List<CardData> networkingCards = new List<CardData>();
    private List<CardData> softwareCards = new List<CardData>();

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

    [ClientRpc]
    public void RpcSetTopic(TOPIC topic)
    {
        this._topic = topic;
    }

    public void LoadCards()
    {
        //if (cardInfos.Count <= 0)
        //{
        //    Debug.Log($"LOADED CARDS: {_topic}");

        //    // Loads cards from .csv
        //    CardData[] loadedCards = ResourceParser.Instance.ParseCSVToCards(_topic);
        //    foreach (CardData card in loadedCards) { cardInfos.Add(card); }
        //}

        computerCards = ResourceParser.Instance.ParseCSVToCards(TOPIC.Computer).ToList();
        networkingCards = ResourceParser.Instance.ParseCSVToCards(TOPIC.Networking).ToList();
        softwareCards = ResourceParser.Instance.ParseCSVToCards(TOPIC.Software).ToList();
    }

    public CardData GetCard(int index)
    {
        switch (_topic)
        {
            case TOPIC.Computer:
                return computerCards[index];
            case TOPIC.Networking:
                return networkingCards[index];
            case TOPIC.Software:
                return softwareCards[index];
        }

        return null;
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

        timer.StopTimer();
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
    }

    [TargetRpc]
    private void TargetDiscard(NetworkConnection conn, GameObject card)
    {
        card.GetComponent<MPDragDrop>().OnDiscard(); // Animator.SetTrigger()
        StartCoroutine(DestroyObjectWithDelay(card));
    }

    #region ------------------------------ UPDATE GUI FUNCTIONS ------------------------------

    [ClientRpc]
    public void RpcUpdateUI(
        GameState gmState,
        byte[] _players,
        byte[] _playerHands,
        string qQuestion,
        string[] qChoices,
        string gameMsg,
        uint currentPlayerId,
        int deckCount,
        byte[] _winners)
    {
        Dictionary<uint, string> players = CustomSerializer.DeserializePlayers(_players) ?? new Dictionary<uint, string>();
        Dictionary<uint, List<int>> playerHands = CustomSerializer.DeserializePlayerHands(_playerHands) ?? new Dictionary<uint, List<int>>();
        Dictionary<uint, string> winners = CustomSerializer.DeserializePlayers(_winners) ?? new Dictionary<uint, string>();

        if (gmState == GameState.FINISHED)
        {
            menuCanvas.ShowEndGameMenu(winners);
        }
        else
        {
            UpdateTurn(
                gmState,
                qQuestion,
                qChoices,
                gameMsg,
                currentPlayerId);
            UpdateOpponentCards(
                gmState,
                currentPlayerId,
                players,
                playerHands);
            UpdateDeckCount(deckCount);
        }

    }

    public void UpdateTurn(
        GameState gmState,
        string qQuestion,
        string[] qChoices,
        string gameMsg,
        uint currentPlayerId)
    {
        isMyTurn = NetworkClient.localPlayer.netId == currentPlayerId &&
            gmState == GameState.STARTED;

        Debug.Log($"ILP: {isMyTurn}, GameState: {gmState}");

        // Disable IP Address when game already started
        if (gmState == GameState.STARTED)
        {
            try
            {
                GameObject.Find("IP-ADDRESS").SetActive(false);
            }
            catch { }
        }

        if (gmState == GameState.QUIZ && qQuestion != null && qChoices != null)
        {
            questionManager.ShowQuestion(qQuestion, qChoices);
            timer.StartQuizTimer();
        }
        else
        {
            if (isMyTurn && gmState == GameState.STARTED)
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
            (isMyTurn && gmState == GameState.STARTED) ?
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
        GameState gmState,
        uint currentPlayerId,
        Dictionary<uint, string> players,
        Dictionary<uint, List<int>> playerHands)
    {
        IEnumerable<KeyValuePair<uint, string>> opponents = players.Where(p => p.Key != NetworkClient.localPlayer.netId);

        //Update UI
        for (int i = 0; i < enemyAreas.transform.childCount; i++)
        {
            TextMeshProUGUI cardCount = enemyAreas.transform.GetChild(i).Find("CardsRemaining").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerNameGO = enemyAreas.transform.GetChild(i).Find("PlayerName").GetComponent<TextMeshProUGUI>();

            try
            {
                uint _opponentId = opponents.ElementAt(i).Key;

                string playerName = players[_opponentId];
                int handCount = playerHands[_opponentId].Count;

                string cardMsg = $"{handCount} Cards";

                playerNameGO.gameObject.GetComponent<TextEllipsisAnimation>().enabled = false;

                if (gmState == GameState.STARTED)
                {
                    // Highlight current player if game started
                    if (_opponentId == currentPlayerId) cardMsg += " (Playing)";

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
            catch
            {
                if (gmState == GameState.STARTED)
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
        card.GetComponent<MPCardInfo>().InitCardData(GetCard(infoIndex), special);
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
        string cardIdToRemove = GetCard(infoIndex).ID;

        for (int i = 0; i < playerArea.transform.childCount; i++)
        {
            GameObject card = playerArea.transform.GetChild(i).gameObject;

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
        timer.StopTimer();
    }

    [Command]
    public void CmdAnswerQuiz(string answer)
    {
        MPGameManager.Instance.OnAnswerQuiz(netIdentity, answer);
    }

    #endregion
}
