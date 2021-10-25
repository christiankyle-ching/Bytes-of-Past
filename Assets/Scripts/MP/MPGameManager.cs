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

    [SyncVar] public float timeLeft = 0f;
    private float timePerTurn = 15f;
    private float minPlayers = 2;

    [SyncVar] public int turns = 0;
    [SyncVar] public bool gameStarted = false;
    [SyncVar] public int currentPlayerIndex = 0;
    [SyncVar] public bool gameFinished = false;
    private SyncList<int> winnersPlayerIndex = new SyncList<int>();
    private SyncList<string> winnerPlayerNames = new SyncList<string>();
    private SyncList<NetworkIdentity> winnerPlayerIdens = new SyncList<NetworkIdentity>();
    private SyncList<NetworkIdentity> players = new SyncList<NetworkIdentity>();
    private int readyPlayersCount = 0;

    private List<CardData> cardInfos = new List<CardData>();
    public SyncList<int> deck = new SyncList<int>(); // indices of card info
    public SyncList<int> timeline = new SyncList<int>(); // indices of card info
    public SyncList<int> playerHands = new SyncList<int>(); // count of hands
    public SyncList<string> playerNames = new SyncList<string>(); // count of hands

    [SyncVar] public TOPIC _topic = TOPIC.Computer;
    private StaticData _staticData;
    private GameObject timer;

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

    public override void OnStartClient()
    {
        base.OnStartClient();

        timer = GameObject.Find("Timer");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _staticData = StaticData.Instance;
        _topic = _staticData.SelectedTopic;

        LoadCards(_topic);
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

    public bool OnPlayCard(int infoIndex, int pos, NetworkIdentity identity, bool hasDrop)
    {
        turns++; // Add Turn

        bool isDropValid;

        if (hasDrop)
        {
            playerHands[FindPlayerIndex(identity)]--; // Decrease Hand Count

            CardData data = cardInfos[infoIndex];

            isDropValid = IsDropValid(data.Year, pos);

            if (isDropValid)
            {
                timeline.Insert(pos, infoIndex);
                Debug.Log($"Player #{identity.netId} [{FindPlayerIndex(identity)}]. CORRECT.");
            }
            else
            {
                Debug.Log($"Player #{identity.netId} [{FindPlayerIndex(identity)}]. WRONG.");

                deck.Add(infoIndex); // Add back the card wrongly placed

                // Add new card to player
                identity.GetComponent<PlayerManager>().CmdGetAnotherCard();
            }
        }
        else
        {
            isDropValid = false;

            // Add new card to player
            identity.GetComponent<PlayerManager>().CmdGetAnotherCard();
        }

        // Calculate next turn then check if starting another round to check winners
        NextPlayerTurn();
        bool isStartRound = turns % playerHands.Count == 0;
        if (isStartRound) CheckWinners();

        ClientsUpdateUI();

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

    public void AddPlayer(NetworkIdentity nid)
    {
        players.Add(nid);
        playerHands.Add(0);
        playerNames.Add($"Unnamed Player #{nid.netId}");

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

    public void StartGame()
    {
        currentPlayerIndex = 0;
        gameStarted = true;
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

        Debug.Log(GetGameStateMessage());
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

    public string GetGameStateMessage()
    {
        if (!gameStarted)
        {
            return $"Waiting for Players to Ready ({readyPlayersCount}/{players.Count})...";
        }
        else if (gameStarted)
        {
            return "ROUND " + ((turns / playerHands.Count) + 1);
        }
        else
        {
            return "";
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
                winnerPlayerIdens.ToArray());
        }
        catch (ArgumentOutOfRangeException)
        {
            // A player has quit, so update UI throws error on array index
        }

    }

    public int FindPlayerIndex(NetworkIdentity i)
    {
        return players.FindIndex(iden => iden == i);
    }

    public void SetPlayerName(NetworkIdentity i, string name)
    {
        if (name != string.Empty)
        {
            int index = FindPlayerIndex(i);
            playerNames[index] = name;
        }
    }

    public void OnPlayerQuit(string playerName)
    {
        NetworkClient.connection.identity.GetComponent<PlayerManager>().RpcGameInterrupted(playerName);
    }

    // ------------------------------ Special Actions ------------------------------

    [Range(0f, 1f)]
    public float specialActionRate = 0.3f; // 0.0 to 1.0 = Percentage of special action

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
}
