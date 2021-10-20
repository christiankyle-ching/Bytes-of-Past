using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class MPGameManager : NetworkBehaviour
{
    private static MPGameManager _instance;
    public static MPGameManager Instance { get { return _instance; } }

    [SyncVar] public bool gameStarted = false;
    private int readyPlayersCount = 0;
    [SyncVar] public NetworkIdentity currentPlayer;
    public SyncList<NetworkIdentity> players = new SyncList<NetworkIdentity>();

    private List<MPCardData> cardInfos = new List<MPCardData>();
    public SyncList<int> deck = new SyncList<int>(); // indices of card info
    public SyncList<int> timeline = new SyncList<int>(); // indices of card info

    [SyncVar] public TOPIC _topic = TOPIC.Computer;

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

        LoadCards(_topic);
    }

    public int PopCard()
    {
        if (deck.Count > 0)
        {
            int id = deck[0];
            deck.RemoveAt(0);
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

    public bool OnPlayCard(int infoIndex, int pos)
    {
        MPCardData data = cardInfos[infoIndex];

        bool isDropValid = IsDropValid(data.year, pos);

        if (isDropValid)
        {
            //Debug.Log($"pos: {pos}");
            //Debug.Log($"infoIndex: {infoIndex}");
            timeline.Insert(pos, infoIndex);
        }

        NextPlayerTurn();

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

    public void CmdAddPlayer(NetworkIdentity i)
    {
        players.Add(i);

        Debug.Log($"Players: {players.Count}. NetID #{i.netId} joined!");
    }

    public void CmdRemovePlayer(NetworkIdentity i)
    {
        players.Remove(i);
    }

    public void AddReadyPlayerCount()
    {
        readyPlayersCount++;

        if (readyPlayersCount >= players.Count && readyPlayersCount >= 2)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        currentPlayer = players[0];

        Debug.Log("Start Game!");
    }

    public void NextPlayerTurn()
    {
        int curPlayerIndex = players.FindIndex(identity => identity.netId == currentPlayer.netId);
        int nextPlayerIndex = curPlayerIndex + 1;

        if (nextPlayerIndex < players.Count)
        {
            currentPlayer = players[nextPlayerIndex];
        }
        else
        {
            currentPlayer = players[0];
        }
    }
}
