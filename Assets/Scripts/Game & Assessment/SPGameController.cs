using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SPGameController : MonoBehaviour
{
    #region
    [Header("Tutorial Mode")]
    bool tutorialMode = false;
    public TutorialManager tutorialManager;

    #endregion
    bool gameEnded = false;

#if UNITY_EDITOR
    int turnSeconds = 5;
    int quizSeconds = 5;
    int quizIntervalRounds = 5;
    int turns = 0;
#else
    // TODO: Set in Prod
    int turnSeconds = 45;
    int quizSeconds = 15;
    int quizIntervalRounds = 5;
    int turns = 0;
#endif


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

    GameDifficulty _difficulty = GameDifficulty.MEDIUM;
    HistoryTopic _topic = HistoryTopic.COMPUTER;

    private List<CardData> deck = new List<CardData>();
    private List<QuestionData> questions = new List<QuestionData>();
    private QuestionData randQuestion;

    public bool gameModeHasQuiz { get => _difficulty == GameDifficulty.MEDIUM || _difficulty == GameDifficulty.HARD; }
    public bool gameModeHasTimer { get => _difficulty == GameDifficulty.HARD; }

    // GAME: Start
    private void Awake()
    {
        tutorialMode = StaticData.Instance.SelectedGameMode == GameMode.TUTORIAL;
        Debug.Log("TUTORIALMODE? " + tutorialMode);

        _difficulty = StaticData.Instance.SelectedDifficulty;
        _topic = StaticData.Instance.SelectedTopic;

        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        LoadCards();
        LoadQuestions();

        if (!tutorialMode)
        {
            tutorialManager.gameObject.SetActive(false);

            switch (_difficulty)
            {
                case GameDifficulty.EASY:
                    InitLives(0);
                    PlaceCardsInTimeline(1);
                    DrawCards(8);
                    break;
                case GameDifficulty.MEDIUM:
                    InitLives(5);
                    PlaceCardsInTimeline(1);
                    DrawCards(8);
                    break;
                case GameDifficulty.HARD:
                    InitLives(3);
                    PlaceCardsInTimeline(3);
                    DrawCards(9);
                    break;
            }

            InitUI();
        }
        else
        {
            _difficulty = GameDifficulty.MEDIUM; // This enables life

            InitLives(5);
            TutorialSetupCards();

            quizIntervalRounds = 3;

            timer.gameObject.SetActive(false);
            questionManager.gameObject.SetActive(true);

            tutorialManager.gameObject.SetActive(true);
        }
    }

    void LoadQuestions()
    {
        questions = ResourceParser.Instance.ParseCSVToQuestions(_topic).ToList();
    }

    void InitUI()
    {
        switch (_difficulty)
        {
            case GameDifficulty.EASY:
                timer.gameObject.SetActive(false);
                questionManager.gameObject.SetActive(false);
                break;
            case GameDifficulty.MEDIUM:
                timer.gameObject.SetActive(false);
                break;
            case GameDifficulty.HARD:
                timer.InitTimer(turnSeconds, quizSeconds);
                timer.StartTimer();
                break;
            default:
                timer.gameObject.SetActive(false);
                questionManager.gameObject.SetActive(false);
                break;
        }
    }

    void InitLives(int count)
    {
        for (int i = 0; i < count; i++) { AddLife(); }

        playerStats.totalLives = count;
    }

    void AddLife()
    {
        playerStats.remainingLives++;
        Instantiate(lifePrefab, playerLivesContainer);
    }

    void DrawCards(int count)
    {
        GiveCard(count);
    }

    void PlaceCardsInTimeline(int count)
    {
        List<CardData> _cards = new List<CardData>();

        _cards.AddRange(PopCards(count));

        if (_cards.Count > 0)
        {
            _cards.Sort((a, b) => a.Year.CompareTo(b.Year));

            foreach (CardData data in _cards)
            {
                SpawnCard(data, dropzone, true);
            }
        }
    }

    // Game Actions
    public void DecreaseLife()
    {
        // If Easy Mode, do not check for decreasing life.
        if (_difficulty == GameDifficulty.EASY) return;

        playerLivesContainer.GetChild(0).GetComponent<Life>().Discard(); // TODO: Problematic

        playerStats.remainingLives--;

        // End the Game
        if (playerStats.remainingLives <= 0)
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

        if (tutorialMode) tutorialManager.NextStep();
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
        if (tutorialMode)
        {
            if (tutorialManager.IsRightDrop(droppedCard.GetComponent<SPCardInfo>().cardData, dropPos))
            {
                tutorialManager.NextStep();
            }
            else
            {
                // Try again
                droppedCard.GetComponent<SPDragDrop>().ResetToOriginalPos();
                return;
            }
        }

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
        if (!tutorialMode)
        {
            randQuestion = questions[UnityEngine.Random.Range(0, questions.Count - 1)];
            questionManager.ShowQuestion(randQuestion);
            RestartTimer(true);
        }
        else
        {
            // Easy Question about Apple Watch
            randQuestion = questions[5];
            questionManager.ShowQuestion(randQuestion);
        }

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
        CardData[] _cards = ResourceParser.Instance.GetCards(_topic);

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
                SpawnCard(cardData, playerArea);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

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

    private void SpawnCard(CardData data, Transform container, bool showYear = false)
    {
        GameObject card = Instantiate(cardPrefab);

        card.GetComponent<SPCardInfo>().cardData = data;
        card.GetComponent<SPCardInfo>().InitCardData(data);

        if (showYear) card.GetComponent<SPDragDrop>().OnPlaceCorrect();

        card.transform.SetParent(container, false);
    }

    private void EndGame(bool isGameWon)
    {
        gameEnded = true;
        timer.StopTimer();
        menuManager.EndGame(isGameWon, playerStats);
    }

    #region Tutorial Methods

    private void TutorialSetupCards()
    {
        CardData[] _cards = ResourceParser.Instance.GetCards(HistoryTopic.COMPUTER);

        // Timeline : ENIAC and iPhone
        CardData[] timelineCards = { _cards[6], _cards[37] };
        foreach (CardData card in timelineCards) SpawnCard(card, dropzone, true);

        // Hand Cards : Apple Watch, TV Typewriter, HP 200A Oscillator 
        CardData[] handCard = { _cards[39], _cards[0], _cards[24] };
        foreach (CardData card in handCard) SpawnCard(card, playerArea);
    }

    #endregion

}
