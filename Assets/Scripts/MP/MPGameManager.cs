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

    public GameObject cardPrefab;

    // TODO: Set in prod
    private int startingCardsCount = 5;
    private int startingTimelineCards = 3;
    private int minPlayers = 2;
    private int quizIntervalRounds = 3;
    private int tradeCount = 2;

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
    public Dictionary<uint, int> playerTrades = new Dictionary<uint, int>();
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
        GenerateDeck();
    }

    private void LoadCards(TOPIC topic)
    {
        CardData[] cards = ResourceParser.Instance.ParseCSVToCards(topic);

        // Load Card Infos
        foreach (CardData data in cards) { cardInfos.Add(data); }
    }

    private void GenerateDeck()
    {
        List<int> tmpDeck = new List<int>();

        // Generate IDs
        for (int i = 0; i < cardInfos.Count; i++)
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

    private void GenerateTimeline()
    {
        List<int> tmpCards = new List<int>();

        // Get Cards
        for (int i = 0; i < startingTimelineCards; i++)
        {
            int cardInfo = PopCard(null);
            tmpCards.Add(cardInfo);
        }

        // Sort then add to timeline
        tmpCards.Sort();
        foreach (int card in tmpCards) { timeline.Add(card); }

        // Show Players
        for (int i = 0; i < tmpCards.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card);
            NetworkClient.localPlayer.GetComponent<PlayerManager>().RpcShowCard(card, tmpCards[i], i, CARDACTION.Played, SPECIALACTION.None);
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
        playerTrades.Add(nid.netId, tradeCount);
        playerHands.Add(nid.netId, new List<int>());

        Debug.Log($"Players: {players.Count}. NetID #{nid.netId} joined!");

        ClientsUpdateUI();
        nid.GetComponent<PlayerManager>().RpcSetTopic(_topic);
    }

    public string RemovePlayer(NetworkIdentity nid)
    {
        players.Remove(nid.netId);
        playerTrades.Remove(nid.netId);
        playerHands.Remove(nid.netId);

        Debug.Log($"Players: {players.Count}. NetID #{nid} left!");

        ClientsUpdateUI();

        return "";
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
        players[iden.netId] = (name != string.Empty) ? name : $"Player#{iden.netId}";
    }

    #endregion

    #region ------------------------------ Game Flow ------------------------------

    public void StartGame()
    {
        currentPlayerIndex = 0;
        gmState = GameState.STARTED;

        GenerateTimeline();
        SetStartingCardsCount();
        foreach (uint _netid in players.Keys)
        {
            NetworkServer.spawned[_netid].GetComponent<PlayerManager>().CmdDrawCards(startingCardsCount);
        }

        Debug.Log("Start Game!");
    }

    public void SetStartingCardsCount()
    {
        switch (players.Count)
        {
            case 2:
                startingCardsCount = 7;
                break;
            case 3:
                startingCardsCount = 6;
                break;
            case 4:
                startingCardsCount = 5;
                break;
        }

#if UNITY_EDITOR
        startingCardsCount = 2;
#endif
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

    public int PopCard(NetworkIdentity iden)
    {
        if (deck.Count > 0)
        {
            int cardId = deck[0];
            deck.RemoveAt(0);

            // If player, else it's timeline
            if (iden != null)
            {
                playerHands[iden.netId].Add(cardId);
            }

            return cardId;
        }

        return -1;
    }

    public List<int> PopCard(NetworkIdentity iden, int count)
    {
        List<int> cards = new List<int>();

        for (int i = 0; i < count; i++)
        {
            if (deck.Count <= 0) break;

            int cardId = deck[0];
            deck.RemoveAt(0);

            cards.Add(cardId);

        }

        playerHands[iden.netId].AddRange(cards);
        return cards;
    }

    public void OnTradeCard(uint p0Id, int p0Card, uint p1Id, int p1Card)
    {
        PlayerManager p0 = NetworkServer.spawned[p0Id].GetComponent<PlayerManager>();
        PlayerManager p1 = NetworkServer.spawned[p1Id].GetComponent<PlayerManager>();

        // Remove card from their hands
        playerHands[p0Id].Remove(p0Card);
        playerHands[p1Id].Remove(p1Card);

        // Add back inversed
        playerHands[p0Id].Add(p1Card);
        playerHands[p1Id].Add(p0Card);

        // Invoke Commands on both players
        p0.TargetDiscardByInfoIndex(p0.connectionToClient, p0Card);
        p0.CmdGetSpecificCard(p1Card);
        p1.TargetDiscardByInfoIndex(p1.connectionToClient, p1Card);
        p1.CmdGetSpecificCard(p0Card);

        // Decrease available trade count
        playerTrades[p0Id]--;

        // Calculate next turn then check if starting another round to check winners
        NextPlayerTurn();

        // Check Winners or Play Quiz on change of rounds
        CheckRound();

        ClientsUpdateUI();

        // Message to all clients
        string fromPlayer = players[p0Id]; // The one who played a card
        string toPlayer = players[p1Id]; // The next person
        ShowTradeToClients(fromPlayer, toPlayer);
    }

    public void ShowTradeToClients(string fromPlayer, string toPlayer)
    {
        string message = "";
        MPGameMessageType type = MPGameMessageType.TRADE;

        message += $"{fromPlayer} has traded a card with {toPlayer}. It's {players.ElementAt(currentPlayerIndex)}'s turn!";

        PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        player.RpcShowGameMessage(message, type);
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

        Debug.Log($"Timeline: {String.Join(",", timeline)}");
        Debug.Log($"{yearBefore}[{dropPos - 1}] - {cardYear} - {yearAfter}[{dropPos}]");
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
        if (hasDrop)
        {
            if (isDropValid)
            {
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
                        type = MPGameMessageType.CORRECT;
                        break;
                    default:
                        type = MPGameMessageType.NONE;
                        break;
                }
            }
            else
            {
                type = MPGameMessageType.WRONG;
            }
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
            int roundNum = (GetCurrentRound() + 1);
            int roundsToNextQuiz = quizIntervalRounds - (roundNum % quizIntervalRounds);
            return $"ROUND {roundNum}. Next quiz in {roundsToNextQuiz} round/s."; // Add 1 because Round is 0-based
        }
        else if (gmState == GameState.QUIZ)
        {
            return $"QUIZ for ROUND {currentRound + 1}."; // Add 1 because Round is 0-based
        }
        else
        {
            return "";
        }
    }

    public int GetCurrentRound()
    {
        return turns / players.Count;
    }

    public void CheckRound()
    {
        int prevRound = currentRound;
        currentRound = GetCurrentRound();

        // If Round has changed
        if (currentRound > prevRound)
        {
            Debug.Log("----- NEXT ROUND -----");

            CheckWinners();

            if ((currentRound + 1) % quizIntervalRounds == 0
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
                CustomSerializer.SerializePlayers(winners),
                CustomSerializer.SerializePlayerTrades(playerTrades));
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
    private float specialActionRate = 0.15f; // 0.0 to 1.0 = Percentage of special action

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
        this.currentQuestion = this.questions[randIndex];
    }

    [Server]
    public void OnAnswerQuiz(NetworkIdentity player, string answer)
    {
        Debug.Log($"QUIZ: Player #{player.netId} answered {answer}");

        quizAnswerCount++;

        if (currentQuestion.isAnswerCorrect(answer))
        {
            List<int> playerHand = playerHands[player.netId];

            if (playerHand.Count > 0)
            {
                int randIndex = UnityEngine.Random.Range(0, playerHand.Count - 1);
                int randCard = playerHand[randIndex];
                player.GetComponent<PlayerManager>().TargetDiscardRandomHand(player.connectionToClient, randCard);
                playerHand.RemoveAt(randIndex);
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
            // TODO: Evaluate when players have another chance even when someone won the game in quiz
            // Temp Fix
            CheckWinners();
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
