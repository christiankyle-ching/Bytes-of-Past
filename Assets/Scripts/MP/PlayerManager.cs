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
    private GameObject menuCanvas;
    private GameObject gameStateText;
    private GameObject deck;
    private GameObject dropzone;

    private List<MPCardData> cardInfos = new List<MPCardData>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerArea");
        enemyAreas = GameObject.Find("EnemyAreas");
        gameStateText = GameObject.Find("RoundText");
        menuCanvas = GameObject.Find("MenuCanvas");
        deck = GameObject.Find("Deck");
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
    public void CmdReady()
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

        // Shouldn't this be getting hold of identity of Player?
        // Result is always the same.
        NetworkIdentity ni = connectionToClient.identity;
        MPGameManager.Instance.SetPlayerName(ni, PlayerPrefs.GetString("Profile_Name", ""));
    }

    public void GetAnotherCard(NetworkIdentity netIdentity)
    {
        int infoIndex = MPGameManager.Instance.PopCard(netIdentity);

        if (infoIndex < 0) return; // if deck has no cards

        GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(card, connectionToClient);
        RpcShowCard(card, infoIndex, -1, CARDACTION.Dealt);
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
            // FIXME: Not working in client
            card.GetComponent<MPDragDrop>().OnDiscard(); // Animator.SetTrigger()
            StartCoroutine(DestroyObjectWithDelay(card));

            GetAnotherCard(netIdentity); // Get Another Card
        }
    }

    #region ------------------------------ UPDATE GUI FUNCTIONS ------------------------------

    [ClientRpc]
    public void RpcUpdateUI(NetworkIdentity currentPlayer, int currentPlayerIndex, NetworkIdentity[] players, int[] playerHands, string[] playerNames, int deckCount, bool gameFinished, string gameMsg, string[] winnerNames)
    {
        if (gameFinished)
        {
            ShowEndGameMenu(winnerNames);
        }

        UpdateTurn(currentPlayer, gameFinished, gameMsg);
        UpdateOpponentCards(currentPlayer, currentPlayerIndex, players, playerHands, playerNames);
        UpdateDeckCount(deckCount);
    }

    public void UpdateTurn(NetworkIdentity identity, bool gameFinished, string gameMsg)
    {
        isMyTurn = identity.isLocalPlayer;

        Debug.Log($"ILP: {isMyTurn}");

        // Enable or disable cards
        if (isMyTurn && !gameFinished)
        {
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

        gameStateText.GetComponent<TextMeshProUGUI>().text = gameMsg;
    }

    public void UpdateOpponentCards(NetworkIdentity currentPlayer, int currentPlayerIndex, NetworkIdentity[] players, int[] playerHands, string[] playerNames)
    {
        List<string> tmpOpponentNames = new List<string>();
        List<int> tmpOpponentHands = new List<int>();

        int opponentCurrentIndex = -1;

        Debug.Log("ILP Cards: " + currentPlayer.isLocalPlayer);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer) continue;

            // .Count because the next index would be the current count before adding the elements
            if (currentPlayerIndex == i) opponentCurrentIndex = tmpOpponentHands.Count;

            tmpOpponentNames.Add(playerNames[i]);
            tmpOpponentHands.Add(playerHands[i]);
        }

        for (int i = 0; i < tmpOpponentHands.Count; i++)
        {
            Transform cardCount = enemyAreas.transform.GetChild(i).Find("CardsRemaining");
            Transform playerName = enemyAreas.transform.GetChild(i).Find("PlayerName");

            string cardMsg = tmpOpponentHands[i].ToString() + " Cards Remaining";
            if (opponentCurrentIndex == i) cardMsg += " (Playing)";

            cardCount.GetComponent<TextMeshProUGUI>().text = cardMsg;
            playerName.GetComponent<TextMeshProUGUI>().text = tmpOpponentNames[i];
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

    public void ShowEndGameMenu(string[] winnerNames)
    {
        Transform bg = menuCanvas.transform.GetChild(0);
        Transform endGame = menuCanvas.transform.GetChild(1);

        bg.gameObject.SetActive(true);
        endGame.gameObject.SetActive(true);

        endGame.Find("WinnerList").GetComponent<TextMeshProUGUI>().text = String.Join("\n", winnerNames);
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
