using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
using TMPro;

public enum GameState
{
    WAITING, STARTED, QUIZ, FINISHED
}

public class MPGameManager : NetworkBehaviour
{
    private static MPGameManager _instance;
    public static MPGameManager Instance { get { return _instance; } }

    // TODO: Set in prod
    private float minPlayers = 2;
    private float quizIntervalRounds = 3;

    public GameState gmState = GameState.WAITING;
    public int turns = 0;
    public int currentRound = 0;
    public int currentPlayerIndex = 0;

    private int readyPlayersCount = 0;

    private List<CardData> cardInfos = new List<CardData>();
    private List<QuestionData> questions = new List<QuestionData>();

    private QuestionData currentQuestion;
    public int quizAnswerCount = 0;

    public List<int> deck = new List<int>(); // indices of card info
    public List<int> timeline = new List<int>(); // indices of card info

    public Dictionary<uint, string> winners = new Dictionary<uint, string>();
    public Dictionary<uint, string> players = new Dictionary<uint, string>();
    public Dictionary<uint, List<int>> playerHands = new Dictionary<uint, List<int>>();

    public TOPIC _topic = TOPIC.Computer;
    private StaticData _staticData;

    #region ------------------------------ Init ------------------------------

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _staticData = StaticData.Instance;
        _topic = _staticData.SelectedTopic;

        LoadCards(_topic);
        LoadQuestions(_topic);
    }

    private void LoadCards(TOPIC topic)
    {
        CardData[] cards = ResourceParser.Instance.ParseCSVToCards(topic);

        List<int> tmpDeck = new List<int>();

        // Load Card Infos
        foreach (CardData data in cards)
        {
            cardInfos.Add(data);
        }

        // Generate IDs
        for (int i = 0; i < cards.Length; i++)
        {
            tmpDeck.Add(i);
        }

        // Shuffle
        IEnumerable shuffledDeck = tmpDeck.OrderBy(x => UnityEngine.Random.Range(0f, 1f));
        foreach (int id in shuffledDeck)
        {
            deck.Add(id);
        }
    }

    private void LoadQuestions(TOPIC topic)
    {
        QuestionData[] questions = ResourceParser.Instance.ParseCSVToQuestions(topic);

        foreach (QuestionData q in questions)
        {
            this.questions.Add(q);
        }
    }


    #endregion

    #region ------------------------------ Player Actions ------------------------------

    public void AddPlayer(NetworkIdentity nid)
    {
        players.Add(nid.netId, "Not Ready");
        playerHands.Add(nid.netId, new List<int>());


        Debug.Log($"Players: {players.Count}. NetID #{nid.netId} joined!");

        ClientsUpdateUI();
    }

    public string RemovePlayer(NetworkIdentity nid)
    {
        players.Remove(nid.netId);
        playerHands.Remove(nid.netId);

        Debug.Log($"Players: {players.Count}. NetID #{nid} left!");

        ClientsUpdateUI();

        return "";
        //return playerName ?? "";
    }

    public void ReadyPlayer(NetworkIdentity ni)
    {
        readyPlayersCount++;

        if (readyPlayersCount >= players.Count && players.Count >= minPlayers)
        {
            StartGame();
        }

        ClientsUpdateUI();

        Debug.Log($"Ready Players: {readyPlayersCount}. Players: {String.Join(",", players)}");
    }

    public void SetPlayerName(NetworkIdentity iden, string name)
    {
        players[iden.netId] = (name != string.Empty) ? name : $"Unnamed Player #{iden.netId}";
    }

    #endregion

    #region ------------------------------ Game Flow ------------------------------

    public void StartGame()
    {
        currentPlayerIndex = 0;
        gmState = GameState.STARTED;

        Debug.Log("Start Game!");
    }

    public void NextPlayerTurn()
    {
        Debug.Log("--- NEXT TURN ---");

        int nextPlayerIndex = currentPlayerIndex + 1;

        if (nextPlayerIndex < players.Count)
        {
            currentPlayerIndex = nextPlayerIndex;
        }
        else
        {
            currentPlayerIndex = 0;
        }

        turns++;
    }

    public void CheckWinners()
    {
        // Get Winners Indices
        foreach (KeyValuePair<uint, string> player in players)
        {
            if (playerHands[player.Key].Count <= 0)
            {
                winners.Add(player.Key, player.Value);
            }
        }

        Debug.Log("Checking Winners: " + winners.Count);

        if (winners.Count > 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        gmState = GameState.FINISHED;
        Debug.Log("End Game!");
    }

    public int PopCard(NetworkIdentity i)
    {
        if (deck.Count > 0)
        {
            int cardId = deck[0];
            deck.RemoveAt(0);
            playerHands[i.netId].Add(cardId);
            return cardId;
        }

        return -1;
    }

    public bool OnPlayCard(int infoIndex, int pos, NetworkIdentity iden, bool hasDrop, SPECIALACTION special)
    {
        bool isDropValid;

        if (hasDrop)
        {
            playerHands[iden.netId].Remove(infoIndex);

            CardData data = cardInfos[infoIndex]; // Get Local copy of data
            isDropValid = IsDropValid(data.Year, pos);

            if (isDropValid)
            {
                timeline.Insert(pos, infoIndex);
                ApplySpecialAction(special, iden);
            }
            else
            {
                deck.Add(infoIndex); // Add back the card wrongly placed

                // Add new card to player
                iden.GetComponent<PlayerManager>().CmdGetAnotherCard();
                if (doubleDrawActive)
                {
                    iden.GetComponent<PlayerManager>().CmdGetAnotherCard();
                    doubleDrawActive = false;
                }

            }
        }
        else
        {
            isDropValid = false;

            // Add new card to player
            iden.GetComponent<PlayerManager>().CmdGetAnotherCard();
            if (doubleDrawActive)
            {
                iden.GetComponent<PlayerManager>().CmdGetAnotherCard();
                doubleDrawActive = false;
            }
        }

        // Calculate next turn then check if starting another round to check winners
        NextPlayerTurn();
        if (skipTurnActive)
        {
            // If Skip Turn is activated, skip another turn
            NextPlayerTurn();
            skipTurnActive = false;
        }

        // Check Winners or Play Quiz on change of rounds
        CheckRound();

        ClientsUpdateUI();

        // Message to all clients
        string thisTurnPlayer = players[iden.netId]; // The one who played a card
        string nextTurnPlayer = players.ElementAt(currentPlayerIndex).Value; // The next person
        ShowMessageToClients(thisTurnPlayer, nextTurnPlayer, special, hasDrop, isDropValid, doubleDrawActive);

        return isDropValid;
    }

    private bool IsDropValid(int year, int dropPos)
    {
        // Shorten code
        int
            yearBefore,
            cardYear,
            yearAfter;

        cardYear = year;
        try
        {
            yearBefore = cardInfos[timeline[dropPos - 1]].Year;
        }
        catch
        {
            yearBefore = int.MinValue;
        }

        try
        {
            yearAfter = cardInfos[timeline[dropPos]].Year;
        }
        catch
        {
            yearAfter = int.MaxValue;
        }

        //Debug.Log(yearBefore + ", " + cardYear + ", " + yearAfter);
        return (yearBefore <= cardYear && cardYear <= yearAfter);
    }

    #endregion

    #region ------------------------------ Remote Calls ------------------------------

    public void ShowMessageToClients(string thisTurnPlayer, string nextTurnPlayer, SPECIALACTION special, bool hasDrop, bool isDropValid, bool doubleDrawActive)
    {
        string message = "";
        MPGameMessageType type = MPGameMessageType.NONE;

        // Build Message String
        if (hasDrop)
        {
            message += $"Player {thisTurnPlayer} placed a card {(isDropValid ? "correct" : "wrong")}! ";
            message += (special != SPECIALACTION.None && isDropValid) ? $"A special action has been activated \"{MPCardInfo.GetSpecialActionLabel(special)}\". " : "";
        }
        else
        {
            message += $"Player {thisTurnPlayer} has skipped their turn! ";
        }

        message += $"It's {nextTurnPlayer}'s turn.";

        // Pick Type of Message
        switch (special)
        {
            case SPECIALACTION.Peek:
                type = MPGameMessageType.SA_PEEK;
                break;
            case SPECIALACTION.SkipTurn:
                type = MPGameMessageType.SA_SKIP;
                break;
            case SPECIALACTION.DoubleDraw:
                type = MPGameMessageType.SA_DOUBLE;
                break;
            case SPECIALACTION.None:
                type = isDropValid ? MPGameMessageType.CORRECT : MPGameMessageType.WRONG;
                break;
            default:
                type = MPGameMessageType.NONE;
                break;
        }

        PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        player.RpcShowGameMessage(message, type);
        player.RpcShowDoubleDraw(doubleDrawActive);
    }

    public string GetGameStateMessage()
    {
        if (gmState != GameState.STARTED)
        {
            return $"Waiting for Players to Ready ({readyPlayersCount}/{players.Count})...";
        }
        else if (gmState == GameState.STARTED)
        {
            return "ROUND " + GetCurrentRound();
        }
        else
        {
            return "";
        }
    }

    public int GetCurrentRound()
    {
        return ((turns / players.Count) + 1);
    }

    public void CheckRound()
    {
        int prevRound = currentRound;
        currentRound = GetCurrentRound();

        // If Round has changed
        if (currentRound > prevRound)
        {
            CheckWinners();

            if (currentRound % quizIntervalRounds == 0
                && gmState != GameState.FINISHED && gmState == GameState.STARTED)
            {
                PlayQuiz();
            }
        }
    }

    public void ClientsUpdateUI()
    {
        try
        {
            PlayerManager pm = NetworkClient.localPlayer.GetComponent<PlayerManager>();

            pm.RpcUpdateUI(
                gmState,
                CustomSerializer.SerializePlayers(players),
                CustomSerializer.SerializePlayerHands(playerHands),
                currentQuestion?.Question,
                currentQuestion?.Choices,
                GetGameStateMessage(),
                players.ElementAt(currentPlayerIndex).Key,
                deck.Count,
                CustomSerializer.SerializePlayers(winners)
                );
        }
        catch (ArgumentOutOfRangeException)
        {
            // A player has quit, so update UI throws error on array index
        }

    }

    #endregion

    #region ------------------------------ Utils ------------------------------

    public void OnPlayerQuit(string playerName, bool isHost = false)
    {
        NetworkClient.connection.identity.GetComponent<PlayerManager>().RpcGameInterrupted(playerName, isHost);
    }
    #endregion

    #region ------------------------------ Special Actions ------------------------------

    // TODO: Set in Prod
    [Range(0f, 1f)]
    private float specialActionRate = 0.3f; // 0.0 to 1.0 = Percentage of special action

    public SPECIALACTION GetRandomSpecialAction()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand <= specialActionRate)
        {
            Array values = Enum.GetValues(typeof(SPECIALACTION));
            System.Random random = new System.Random();
            return (SPECIALACTION)values.GetValue(random.Next(values.Length - 1)); // -1 because last special action is None
        }
        else
        {
            return SPECIALACTION.None;
        }
    }

    public bool doubleDrawActive = false;
    public bool skipTurnActive = false;

    private void ApplySpecialAction(SPECIALACTION special, NetworkIdentity player)
    {
        if (special == SPECIALACTION.None) return;

        Debug.Log($"Apply Special: {special}, Activated By: #{player.netId}");
        switch (special)
        {
            case SPECIALACTION.Peek:
                player.GetComponent<PlayerManager>().TargetPeekCard(player.connectionToClient);
                break;
            case SPECIALACTION.DoubleDraw:
                doubleDrawActive = true;
                break;
            case SPECIALACTION.SkipTurn:
                skipTurnActive = true;
                break;
        }
    }

    #endregion

    #region ------------------------------ Quiz Round ------------------------------

    public void PlayQuiz()
    {
        gmState = GameState.QUIZ;

        int randIndex = UnityEngine.Random.Range(0, this.questions.Count - 1);
        //this.currentQuestion = this.questions[randIndex];
        this.currentQuestion = this.questions[0];
    }

    public void OnAnswerQuiz(NetworkIdentity player, string answer)
    {
        quizAnswerCount++;

        if (currentQuestion.isAnswerCorrect(answer))
        {
            List<int> playerHand = playerHands[player.netId];

            if (playerHand.Count > 0)
            {
                int randCard = playerHand[UnityEngine.Random.Range(0, playerHand.Count - 1)];
                player.GetComponent<PlayerManager>().TargetDiscardRandomHand(player.connectionToClient, randCard);
            }

            player.GetComponent<PlayerManager>().TargetShowQuizResult(
                player.connectionToClient,
                "Your answer is correct! A random card has been discarded.",
                MPGameMessageType.CORRECT);
        }
        else
        {
            player.GetComponent<PlayerManager>().TargetShowQuizResult(
                player.connectionToClient,
                (answer == string.Empty) ?
                    "Sorry! You ran out of time." :
                    "Sorry! Your answer is wrong.",
                MPGameMessageType.WRONG);
        }

        if (quizAnswerCount >= players.Count)
        {
            EndQuiz();
            ClientsUpdateUI();
        }
    }

    private void EndQuiz()
    {
        quizAnswerCount = 0;
        gmState = GameState.STARTED;
        currentQuestion = null;
    }

    #endregion
}
