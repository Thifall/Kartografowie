using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiscoveryDeckManager : MonoBehaviour
{
    public List<DiscoveryCard> deck; // Lista wszystkich kart, punkt wyjœcia sezonu
    public SeasonManager seasonManager;
    public CardDrawEventSO cardDrawEvent;
    public ShapeSelectedEventSO shapeSelectedEvent;
    public TerrainSelectedEventSO terrainSelectedEvent;
    public GameObject NormalCardPrefab;
    public GameObject RuinsCardPrefab;
    public Transform CardStackParent;

    private List<DiscoveryCard> seasonDeck; //u¿ywany do manipulacji tali¹ podczas rozgrywki
    private List<GameObject> activeCards = new List<GameObject>();
    private bool gameOver = false;
    private DiscoveryCard currentCard;

    void Start()
    {
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        ClearCardStack();
        if (gameOver)
        {
            return;
        }
        seasonDeck = new List<DiscoveryCard>(deck.OrderBy(c => Random.value)); // Tasowanie kart
    }

    private void ClearCardStack()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();
    }

    public void DrawCard()
    {

        if (!gameOver && seasonManager.IsSeasonOver()) // this has to be first, cause season wrapping mechanics push gameOver notification
        {
            seasonManager.NextSeason();
        }
        if (gameOver || seasonDeck.Count == 0)
        {
            return;
        }
        if (!FindFirstObjectByType<ShapePreview>().WasShapePlaced())
        {
            return;
        }
        currentCard = seasonDeck[0];
        seasonDeck.RemoveAt(0);
        ApplyCardEffects(currentCard);
        CreateNewCardUI();
        cardDrawEvent.RaiseEvent(currentCard);
        if (!currentCard.IsRuins)
        {
            shapeSelectedEvent.RaiseEvent(currentCard.ShapeIcons[0]);
            terrainSelectedEvent.RaiseEvent(currentCard.availableTerrains[0]);
        }
    }

    private void CreateNewCardUI()
    {
        GameObject cardUI = Instantiate(currentCard.IsRuins ? RuinsCardPrefab : NormalCardPrefab, CardStackParent);
        cardUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40 * activeCards.Count);
        cardUI.GetComponent<CardUI>().SetupCardUI(currentCard);
        activeCards.Add(cardUI);
    }

    void ApplyCardEffects(DiscoveryCard card)
    {
        if (seasonDeck.Count < 1)
        {
            BlockDrawButton();
        }
        //season manager update -> should react to event
        seasonManager.ProgressSeason(card.TimeValue);
    }

    private void BlockDrawButton()
    {
        gameObject.GetComponent<Image>().color = Color.red;
    }

    public void OnSeasonEnd(bool gameEnded)
    {
        gameOver = gameEnded;
        // Mo¿esz dodaæ logikê np. dodawania specjalnych kart przed tasowaniem
        if (gameOver)
        {
            BlockDrawButton();
            return;
        }
        ShuffleDeck();
    }
}
