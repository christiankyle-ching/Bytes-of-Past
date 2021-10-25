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
    private float timePerTurn = 5f;
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

    private List<MPCardData> cardInfos = new List<MPCardData>();
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

    private void Update()
    {
        // TODO: FIXME
        //UpdateTimer();
    }

    public void UpdateTimer()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0 && gameStarted && !gameFinished)
        {
            // throws NullReferenceException
            players[currentPlayerIndex].GetComponent<PlayerManager>().CmdGetAnotherCard();
            SetTimerText(timeLeft);
            ResetTimer();
            NextPlayerTurn();
        }
    }

    public void SetTimerText(float time)
    {
        timer.GetComponentInChildren<TextMeshProUGUI>().text = $"{Math.Ceiling(time)} S";
    }

    public void ResetTimer()
    {
        timeLeft = timePerTurn;
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
                        >("Cards/Cards - Software");
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

        List<int> tmpDeck = new List<int>();
        // Load Card Infos
        foreach (MPCardData data in cards)
        {
            cardInfos.Add(data);
        }

        // Generate IDs
        for (int i = 0; i < cards.Count; i++)
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

            MPCardData data = cardInfos[infoIndex];

            isDropValid = IsDropValid(data.year, pos);

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
            yearBefore = cardInfos[timeline[dropPos - 1]].year;
        }
        catch
        {
            yearBefore = int.MinValue;
        }

        try
        {
            yearAfter = cardInfos[timeline[dropPos]].year;
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
        ResetTimer();
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

        ResetTimer();
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
}
