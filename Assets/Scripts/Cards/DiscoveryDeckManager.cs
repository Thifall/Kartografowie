using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoveryDeckManager : MonoBehaviour
{
    public List<DiscoveryCard> deck; // Lista wszystkich kart, punkt wyjœcia sezonu
    private List<DiscoveryCard> seasonDeck; //u¿ywany do manipulacji tali¹ podczas rozgrywki
    public SeasonManager seasonManager;
    private DiscoveryCard currentCard;
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
    }

    void ApplyCardEffects(DiscoveryCard card)
    {
        Debug.Log($"Wylosowano kartê: {card.CardName}");
        FindFirstObjectByType<ShapeSelector>().SetAvailableShapes(card.availableShapes);
        FindFirstObjectByType<ShapePreview>().SetAvailableTerrains(card.availableTerrains);

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
