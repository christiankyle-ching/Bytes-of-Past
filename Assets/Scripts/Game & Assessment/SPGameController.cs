using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SPGameController : MonoBehaviour
{
    int playerLives = 0;
    int playerLivesLeft = 0;
    bool gameEnded = false;

    // TODO: Set in Prod
    int startingCardsCount = 8;
    int turnSeconds = 45;
    int quizSeconds = 15;
    int quizIntervalRounds = 5;
    int turns = 0;

    [Header("UI References")]
    public Transform playerArea;
    public Transform dropzone;
    public SPQuestionManager questionManager;
    public MPGameMessage messenger;
    public PlayerStats playerStats;
    public Transform playerLivesContainer;
    public SinglePlayerMenuManager menuManager;
    public SPTimer timer;

    [Header("Prefabs")]
    public GameObject lifePrefab;
    public GameObject cardPrefab;

    private StaticData staticData;
    DIFFICULTY _difficulty = DIFFICULTY.Medium;
    TOPIC _topic = TOPIC.Computer;

    private List<CardData> deck = new List<CardData>();
    private List<QuestionData> questions = new List<QuestionData>();
    private QuestionData randQuestion;

    public bool gameModeHasQuiz { get => _difficulty == DIFFICULTY.Medium || _difficulty == DIFFICULTY.Hard; }
    public bool gameModeHasTimer { get => _difficulty == DIFFICULTY.Hard; }

    // GAME: Start
    private void Awake()
    {
        staticData = StaticData.Instance;
        _difficulty = StaticData.Instance.SelectedDifficulty;
        _topic = StaticData.Instance.SelectedTopic;

        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        switch (_difficulty)
        {
            case DIFFICULTY.Easy:
                playerLives = 0;
                startingCardsCount = 8;
                break;
            case DIFFICULTY.Medium:
                playerLives = 5;
                startingCardsCount = 8;
                break;
            case DIFFICULTY.Hard:
                playerLives = 3;
                startingCardsCount = 8;
                break;
        }

        playerLivesLeft = playerLives;

        playerStats.initialLife = playerLives;
        playerStats.remainingLife = playerLivesLeft;

        LoadCards();
        LoadQuestions();

        InitLives();
        PlaceCardsInTimeline();
        DrawCards();

        InitUI();
    }

    void LoadQuestions()
    {
        questions = ResourceParser.Instance.ParseCSVToQuestions(_topic).ToList();
    }

    void InitUI()
    {
        switch (_difficulty)
        {
            case DIFFICULTY.Easy:
                timer.gameObject.SetActive(false);
                questionManager.gameObject.SetActive(false);
                break;
            case DIFFICULTY.Medium:
                timer.gameObject.SetActive(false);
                break;
            case DIFFICULTY.Hard:
                timer.InitTimer(turnSeconds, quizSeconds);
                timer.StartTimer();
                break;
            default:
                timer.gameObject.SetActive(false);
                questionManager.gameObject.SetActive(false);
                break;
        }
    }

    void InitLives()
    {
        for (int i = 0; i < playerLives; i++) { AddLife(); }
    }

    void AddLife()
    {
        playerStats.remainingLife++;
        Instantiate(lifePrefab, playerLivesContainer);
    }

    void DrawCards()
    {
        GiveCard(startingCardsCount);
    }

    void PlaceCardsInTimeline()
    {
        List<CardData> _cards = new List<CardData>();

        switch (_difficulty)
        {
            case DIFFICULTY.Easy:
                _cards.AddRange(PopCards(1));
                break;
            case DIFFICULTY.Medium:
                _cards.AddRange(PopCards(1));
                break;
            case DIFFICULTY.Hard:
                _cards.AddRange(PopCards(3));
                break;
        }

        if (_cards.Count > 0)
        {
            _cards.Sort((a, b) => a.Year.CompareTo(b.Year));

            foreach (CardData data in _cards)
            {
                GameObject cardGO = Instantiate(cardPrefab, dropzone);
                SPCardInfo card = cardGO.GetComponent<SPCardInfo>();

                card.cardData = data;
                card.InitCardData(data);

                cardGO.GetComponent<SPDragDrop>().OnPlaceCorrect();
            }
        }
    }

    // Game Actions
    public void DecreaseLife()
    {
        // If Easy Mode, do not check for decreasing life.
        if (_difficulty == DIFFICULTY.Easy) return;

        playerLivesContainer.GetChild(0).GetComponent<Life>().Discard(); // TODO: Problematic

        playerLivesLeft--;
        playerStats.remainingLife = playerLivesLeft;

        // If the index is 0 (which means that the destroyed life is the last one)
        // End the Game
        if (playerLivesLeft <= 0)
        {
            EndGame(false);
        }
    }

    // Game Flow Functions
    public void AnswerQuiz(string answer)
    {
        if (randQuestion.isAnswerCorrect(answer))
        {
            messenger.ShowMessage("Your answer is correct! A life has been added.", MPGameMessageType.CORRECT);
            AddLife();
        }
        else
        {
            messenger.ShowMessage(
                answer == string.Empty ? "Sorry! You ran out of time."
                : "Sorry! Your answer is wrong."
                , MPGameMessageType.WRONG);
        }

        questionManager.SetVisibility(false);
        RestartTimer();
    }

    private void RestartTimer(bool isQuiz = false)
    {
        if (!gameEnded)
        {
            if (gameModeHasTimer)
            {
                if (gameModeHasQuiz)
                {
                    if (!isQuiz)
                    {
                        timer.StartTimer();
                    }
                    else
                    {
                        timer.StartQuizTimer();
                    }
                }
                else
                {
                    timer.StartTimer();
                }
            }
        }
    }

    public void HandleDropInTimeline(GameObject droppedCard, int dropPos)
    {
        turns++;

        if (droppedCard != null)
        {
            if (IsDropValid(droppedCard.GetComponent<SPCardInfo>().cardData, dropPos))
            {
                droppedCard.GetComponent<SPDragDrop>().ReplacePlaceholder();
                droppedCard.GetComponent<SPDragDrop>().OnPlaceCorrect();
                playerStats.CorrectDrop();

                messenger.ShowMessage("Good Job! That's correct.", MPGameMessageType.CORRECT);

                if (IsHandEmpty()) EndGame(true);
            }
            else
            {
                messenger.ShowMessage("Oops! That's wrong.", MPGameMessageType.WRONG);
                HandleInvalidDrop(droppedCard);
                playerStats.IncorrectDrop();
            }
        }
        else
        {
            DecreaseLife();
            messenger.ShowMessage("Oops! You ran out of time.", MPGameMessageType.WRONG);
        }

        if (gameModeHasQuiz)
        {
            if (turns % quizIntervalRounds == 0)
            {
                PlayQuiz();
            }
            else
            {
                RestartTimer();
            }
        }
    }

    private void PlayQuiz()
    {
        randQuestion = questions[UnityEngine.Random.Range(0, questions.Count - 1)];
        questionManager.ShowQuestion(randQuestion);
        RestartTimer(true);
    }

    private bool IsHandEmpty()
    {
        // GetChild is necessary since cards are placed in a container inside a DropZone
        return playerArea.childCount <= 0;
    }

    private void HandleInvalidDrop(GameObject droppedCard)
    {
        DecreaseLife();

        if (droppedCard != null)
        {
            deck.Add(droppedCard.GetComponent<SPCardInfo>().cardData);
            droppedCard.GetComponent<SPDragDrop>().OnDiscard();
        }

        try
        {
            GiveCard(1);
        }
        catch (InvalidOperationException)
        {
            // No cards left?
        }
    }

    private bool IsDropValid(CardData droppedCard, int dropPos)
    {
        if (droppedCard == null) return false;

        SPCardInfo[] timelineCards =
            dropzone.GetComponentsInChildren<SPCardInfo>();

        // Shorten code
        int
            yearBefore,
            cardYear,
            yearAfter;

        cardYear = droppedCard.Year;
        try
        {
            yearBefore = timelineCards[dropPos - 1].cardData.Year;
        }
        catch
        {
            yearBefore = int.MinValue;
        }

        try
        {
            yearAfter = timelineCards[dropPos].cardData.Year;
        }
        catch
        {
            yearAfter = int.MaxValue;
        }

        Debug.Log(yearBefore + ", " + cardYear + ", " + yearAfter);
        return (yearBefore <= cardYear && cardYear <= yearAfter);
    }

    void LoadCards()
    {
        CardData[] _cards = ResourceParser.Instance.ParseCSVToCards(_topic);

        // Shuffle
        IEnumerable shuffledDeck = _cards.OrderBy(x => UnityEngine.Random.Range(0f, 1f));

        foreach (CardData card in shuffledDeck) deck.Add(card);
    }

    public CardData[] PopCards(int count)
    {
        List<CardData> _cards = new List<CardData>();

        try
        {
            for (int i = 0; i < count; i++)
            {
                CardData cardData = deck.ElementAt(0);
                deck.RemoveAt(0);
                _cards.Add(cardData);

                if (deck.Count <= 0) break;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return _cards.ToArray();
    }

    public void GiveCard(int count)
    {
        if (deck.Count <= 0) return;

        try
        {
            CardData[] _cards = PopCards(count);

            foreach (CardData cardData in _cards)
            {
                GameObject card = Instantiate(cardPrefab);
                card.GetComponent<SPCardInfo>().cardData = cardData;
                card.GetComponent<SPCardInfo>().InitCardData(cardData);

                card.transform.SetParent(playerArea, false);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        //SetRemainingCards(); TODO? 

        if (count > 0)
        {
            if (count > 1)
            {
                SoundManager.Instance.PlayMultipleDrawSFX();
            }
            else
            {
                SoundManager.Instance.PlayDrawSFX();
            }
        }
    }

    private void EndGame(bool isGameWon)
    {
        gameEnded = true;
        timer.StopTimer();
        menuManager.EndGame(isGameWon, playerStats);
    }

}
