using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
using TMPro;

public class MPGameManager : NetworkBehaviour
{
    private static MPGameManager _instance;
    public static MPGameManager Instance { get { return _instance; } }

    // TODO: Set in prod
    private float minPlayers = 2;
    private float quizIntervalRounds = 3;

    [SyncVar] public int turns = 0;
    [SyncVar] public int currentRound = 0;
    [SyncVar] public bool gameStarted = false;
    [SyncVar] public int currentPlayerIndex = 0;
    [SyncVar] public bool gameFinished = false;
    private SyncList<int> winnersPlayerIndex = new SyncList<int>();
    private SyncList<string> winnerPlayerNames = new SyncList<string>();
    private SyncList<NetworkIdentity> winnerPlayerIdens = new SyncList<NetworkIdentity>();
    private SyncList<NetworkIdentity> players = new SyncList<NetworkIdentity>();
    private int readyPlayersCount = 0;

    private List<QuestionData> questions = new List<QuestionData>();
    private QuestionData currentQuestion;
    [SyncVar] public bool isQuizActive = false;
    [SyncVar] public int quizAnswerCount = 0;

    private List<CardData> cardInfos = new List<CardData>();
    public SyncList<int> deck = new SyncList<int>(); // indices of card info
    public SyncList<int> timeline = new SyncList<int>(); // indices of card info
    public SyncList<int> playerHands = new SyncList<int>(); // count of hands
    public SyncList<string> playerNames = new SyncList<string>(); // count of hands

    [SyncVar] public TOPIC _topic = TOPIC.Computer;
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
        players.Add(nid);
        playerHands.Add(0);
        //playerNames.Add($"Unnamed Player #{nid.netId}");
        playerNames.Add("Not Ready");

        Debug.Log($"Players: {players.Count} = {playerHands.Count}. NetID #{nid.netId} joined!");
        ClientsUpdateUI();
    }

    public string RemovePlayer(NetworkIdentity nid)
    {
        int index = players.FindIndex(identity => identity == nid);
        string playerName = playerNames[index];

        players.Remove(nid);
        playerHands.RemoveAt(index);
        playerNames.RemoveAt(index);

        Debug.Log($"Players: {players.Count} = {playerHands.Count}. NetID #{nid} left!");
        ClientsUpdateUI();

        return playerName ?? "";
    }

    public void ReadyPlayer(NetworkIdentity ni)
    {
        readyPlayersCount++;

        if (readyPlayersCount >= players.Count && players.Count >= minPlayers)
        {
            StartGame();
        }
        else
        {
            ClientsUpdateUI();
        }

        Debug.Log($"Ready Players: {readyPlayersCount}. Players: {String.Join(",", playerNames)}");
    }

    public void SetPlayerName(NetworkIdentity i, string name)
    {
        if (name != string.Empty)
        {
            int index = FindPlayerIndex(i);
            playerNames[index] = name;
        }
    }

    #endregion

    #region ------------------------------ Game Flow ------------------------------

    public void StartGame()
    {
        currentPlayerIndex = 0;
        gameStarted = true;
        currentRound = GetCurrentRound();

        ClientsUpdateUI();

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
    }

    public void CheckWinners()
    {
        // Get Winners Indices
        for (int i = 0; i < players.Count; i++)
        {
            if (playerHands[i] <= 0)
            {
                winnersPlayerIndex.Add(i);
            }
        }

        // Get their Names
        for (int i = 0; i < players.Count; i++)
        {
            if (winnersPlayerIndex.Contains(i))
            {
                winnerPlayerNames.Add(playerNames[i]);
                winnerPlayerIdens.Add(players[i]);
            }
        }

        Debug.Log("Checking Winners: " + winnersPlayerIndex.Count);

        if (winnersPlayerIndex.Count > 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        gameFinished = true;
        Debug.Log("End Game!");
    }

    public int PopCard(NetworkIdentity i)
    {
        if (deck.Count > 0)
        {
            int id = deck[0];
            deck.RemoveAt(0);

            Debug.Log($"Player #{i.netId} [{FindPlayerIndex(i)}] gets a CARD.");
            playerHands[FindPlayerIndex(i)]++;
            return id;
        }

        return -1;
    }

    public bool OnPlayCard(int infoIndex, int pos, NetworkIdentity identity, bool hasDrop, SPECIALACTION special)
    {
        turns++; // Add Turn

        bool isDropValid;

        if (hasDrop)
        {
            playerHands[FindPlayerIndex(identity)]--; // Decrease Hand Count
            CardData data = cardInfos[infoIndex]; // Get Local copy of data
            isDropValid = IsDropValid(data.Year, pos);

            if (isDropValid)
            {
                timeline.Insert(pos, infoIndex);
                ApplySpecialAction(special, identity);
            }
            else
            {
                deck.Add(infoIndex); // Add back the card wrongly placed

                // Add new card to player
                identity.GetComponent<PlayerManager>().CmdGetAnotherCard();
                if (doubleDrawActive)
                {
                    identity.GetComponent<PlayerManager>().CmdGetAnotherCard();
                    doubleDrawActive = false;
                }

            }
        }
        else
        {
            isDropValid = false;

            // Add new card to player
            identity.GetComponent<PlayerManager>().CmdGetAnotherCard();
            if (doubleDrawActive)
            {
                identity.GetComponent<PlayerManager>().CmdGetAnotherCard();
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
        CheckRound(GetCurrentRound());

        ClientsUpdateUI();

        // Message to all clients
        string thisTurnPlayer = playerNames[FindPlayerIndex(identity)]; // The one who played a card
        string nextTurnPlayer = playerNames[currentPlayerIndex]; // The next person
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
            message += (special != SPECIALACTION.None) ? $"A special action has been activated \"{MPCardInfo.GetSpecialActionLabel(special)}\". " : "";
        }
        else
        {
            message += $"Player {thisTurnPlayer} has skipped their turn! ";
        }
        message += $"It's {nextTurnPlayer}'s turn.";

        // Pick Type of Message
        if (hasDrop)
        {
            if (special == SPECIALACTION.SkipTurn)
            {
                type = MPGameMessageType.SA_SKIP;
            }
            else if (special == SPECIALACTION.Peek)
            {
                type = MPGameMessageType.SA_PEEK;
            }
            else if (isDropValid)
            {
                type = MPGameMessageType.CORRECT;
            }
            else
            {
                type = MPGameMessageType.WRONG;
            }
        }
        else
        {
            type = MPGameMessageType.NONE;
        }

        PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        player.RpcShowGameMessage(message, type);
        player.RpcShowDoubleDraw(doubleDrawActive);
    }

    public string GetGameStateMessage()
    {
        if (!gameStarted)
        {
            return $"Waiting for Players to Ready ({readyPlayersCount}/{players.Count})...";
        }
        else if (gameStarted)
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
        return ((turns / playerHands.Count) + 1);
    }

    public void CheckRound(int newValue)
    {
        if (newValue > currentRound)
        {
            currentRound = newValue;

            CheckWinners();

            if (newValue % quizIntervalRounds == 0 && !gameFinished)
            {
                PlayQuiz();
            }
        }
    }

    public void ClientsUpdateUI()
    {
        try
        {
            PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
            NetworkIdentity currentPlayer = players[currentPlayerIndex];

            player.RpcUpdateUI(
                currentPlayer,
                currentPlayerIndex,
                players.ToArray(),
                playerHands.ToArray(),
                playerNames.ToArray(),
                deck.Count,
                gameFinished,
                GetGameStateMessage(),
                winnerPlayerNames.ToArray(),
                winnerPlayerIdens.ToArray(),
                currentQuestion?.Question,
                currentQuestion?.Choices);
        }
        catch (ArgumentOutOfRangeException)
        {
            // A player has quit, so update UI throws error on array index
        }

    }

    #endregion

    #region ------------------------------ Utils ------------------------------

    public int FindPlayerIndex(NetworkIdentity i)
    {
        return players.FindIndex(iden => iden == i);
    }

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

    [SyncVar] public bool doubleDrawActive = false;
    [SyncVar] public bool skipTurnActive = false;

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
        isQuizActive = true;

        int randIndex = UnityEngine.Random.Range(0, this.questions.Count - 1);
        this.currentQuestion = this.questions[randIndex];
    }

    public void OnAnswerQuiz(NetworkIdentity player, string answer)
    {
        quizAnswerCount++;

        int playerIndex = FindPlayerIndex(player);

        Debug.Log($"Player [{playerIndex}] answered '{answer}'. QuizAnswers: {quizAnswerCount}. currentQuestion: {currentQuestion == null}");

        if (currentQuestion.isAnswerCorrect(answer))
        {
            playerHands[playerIndex]--; // Reduce Hand Count
            player.GetComponent<PlayerManager>().TargetDiscardRandomHand(player.connectionToClient);

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
        isQuizActive = false;
        currentQuestion = null;
    }

    #endregion
}
