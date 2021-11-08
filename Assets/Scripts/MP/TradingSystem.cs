using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class TradingSystem : MonoBehaviour
{
    [Header("Child Elements")]
    public GameObject bg;
    public GameObject mainPanel;
    public GameObject cardPrefab;
    public Button closeButton;
    public Button tradeButton;
    public TextMeshProUGUI title;
    public Image leftArrow;
    public Image rightArrow;

    [Header("Your Player")]
    public TextMeshProUGUI player0NameGO;
    public Transform player0CardsGO;

    [Header("Other Player")]
    public TextMeshProUGUI player1NameGO;
    public Transform player1CardsGO;

    // TODO: Set in Prod
    [Header("Card Highlight")]
    public Color highlightColor = Color.green;
    public Vector2 effectDistance = new Vector2(10f, 10f);

    private uint player0NetId;
    private int selectedCard_player0 = -1;
    private uint player1NetId;
    private int selectedCard_player1 = -1;

    public HistoryTopic _topic;

    void Start()
    {
        SetupCanvas(false);
        tradeButton.onClick.AddListener(() => TradeCards());
        closeButton.onClick.AddListener(() => SetupCanvas(false));
    }

    public void SetTopic(HistoryTopic _topic)
    {
        this._topic = _topic;
    }

    public void SetupCanvas(bool _enabled)
    {
        bg.SetActive(_enabled);
        mainPanel.SetActive(_enabled);
    }

    public void ShowTrade(
        uint player0Id,
        string player0Name,
        int[] player0Cards,
        uint player1Id,
        string player1Name,
        int[] player1Cards,
        int tradesLeft)
    {
        title.text = $"Trade Cards ({tradesLeft} chances remaining)";

        selectedCard_player0 = -1;
        selectedCard_player1 = -1;
        CheckValidTrade();

        player0NetId = player0Id;
        player0NameGO.text = player0Name;
        LoadPlayerCards(player0Cards, player0CardsGO);

        player1NetId = player1Id;
        player1NameGO.text = player1Name;
        LoadPlayerCards(player1Cards, player1CardsGO);

        SetupCanvas(true);
    }

    void LoadPlayerCards(int[] cards, Transform parent)
    {
        ClearContainer(parent);

        foreach (int card in cards)
        {
            InstantiateCard(card, parent);
        }
    }

    void InstantiateCard(int infoIndex, Transform parent)
    {
        GameObject card = Instantiate(cardPrefab);
        card.GetComponent<MPDragDrop>().enabled = false; // Disable drag
        Outline outline = card.AddComponent<Outline>(); // Add Outline
        outline.enabled = false;
        outline.effectColor = highlightColor;
        outline.effectDistance = effectDistance;

        // Load Info
        card.GetComponent<MPCardInfo>().InitCardData(ResourceParser.Instance.GetCard(infoIndex, _topic));
        card.GetComponent<MPCardInfo>().infoIndex = infoIndex;
        card.transform.SetParent(parent, false);

        // Attach Listener
        int index = card.transform.GetSiblingIndex();
        Button btn = card.AddComponent<Button>();
        btn.onClick.AddListener(() => OnCardClick(card));
    }

    void ClearContainer(Transform _parent)
    {
        foreach (Transform obj in _parent) { Destroy(obj.gameObject); }
    }

    void OnCardClick(GameObject card)
    {
        SoundManager.Instance.PlayClickedSFX();

        Transform parent = card.transform.parent;
        ClearHighlightedCard(parent);

        if (parent == player0CardsGO)
        {
            selectedCard_player0 = card.transform.GetSiblingIndex();
        }
        else if (parent == player1CardsGO)
        {
            selectedCard_player1 = card.transform.GetSiblingIndex();
        }

        card.GetComponent<Outline>().enabled = true;

        CheckValidTrade();
    }

    void ClearHighlightedCard(Transform parent)
    {
        foreach (Transform card in parent)
        {
            card.GetComponent<Outline>().enabled = false;
        }
    }

    void CheckValidTrade()
    {
        rightArrow.color = selectedCard_player0 >= 0 ? Color.white : new Color(1, 1, 1, 0.5f);
        leftArrow.color = selectedCard_player1 >= 0 ? Color.white : new Color(1, 1, 1, 0.5f);
        tradeButton.interactable = selectedCard_player0 >= 0 && selectedCard_player1 >= 0;
    }

    void TradeCards()
    {
        SoundManager.Instance.PlayClickedSFX();

        int player0Card = player0CardsGO.GetChild(selectedCard_player0).GetComponent<MPCardInfo>().infoIndex;
        int player1Card = player1CardsGO.GetChild(selectedCard_player1).GetComponent<MPCardInfo>().infoIndex;

        NetworkClient.localPlayer.GetComponent<PlayerManager>().TradeCard(player0NetId, player0Card, player1NetId, player1Card);
    }
}
