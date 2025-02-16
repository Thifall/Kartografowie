using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoveryDeckManager : MonoBehaviour
{
    public List<DiscoveryCard> deck; // Lista wszystkich kart, punkt wyjœcia sezonu
    private List<DiscoveryCard> seasonDeck; //u¿ywany do manipulacji tali¹ podczas rozgrywki
    public SeasonManager seasonManager;
    private DiscoveryCard currentCard;
    public CardDrawEventSO cardDrawEvent;
    public ShapeSelectedEventSO shapeSelectedEvent;
    public TerrainSelectedEventSO terrainSelectedEvent;
    private bool gameOver = false;

    void Start()
    {
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        if (gameOver)
        {
            return;
        }
        seasonDeck = new List<DiscoveryCard>(deck.OrderBy(c => Random.value)); // Tasowanie kart
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

        cardDrawEvent.RaiseEvent(currentCard);
        shapeSelectedEvent.RaiseEvent(currentCard.ShapeIcons[0]);
        terrainSelectedEvent.RaiseEvent(currentCard.availableTerrains[0]);
    }

    void ApplyCardEffects(DiscoveryCard card)
    {
        if (seasonDeck.Count < 1)
        {
            BlockDrawButton();
        }
        //season manager update
        seasonManager.ProgressSeason(card.TimeValue);
    }

    private void BlockDrawButton()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
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

    private void OnMouseDown()
    {
        DrawCard();
    }
}
