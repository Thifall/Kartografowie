using Kartografowie.Cards.UI;
using Kartografowie.General;
using Kartografowie.Shapes;
using Kartografowie.Terrains;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kartografowie.Cards
{
    public class DiscoveryDeckManager : MonoBehaviour
    {
        public List<DiscoveryCard> deck; // Lista wszystkich kart odkryæ, punkt wyjœcia sezonu
        public List<AmbushCard> ambushDeck; // List kart zasadzki
        public SeasonManager seasonManager;
        public CardDrawEventSO cardDrawEvent;
        public ShapeSelectedEventSO shapeSelectedEvent;
        public TerrainSelectedEventSO terrainSelectedEvent;
        public GameObject NormalCardPrefab;
        public GameObject RuinsCardPrefab;
        public Transform CardStackParent;

        private List<DiscoveryCard> seasonDeck = new(); //u¿ywany do manipulacji tali¹ podczas rozgrywki
        private readonly List<GameObject> activeCards = new();
        private bool gameOver = false;
        private DiscoveryCard currentCard;

        void Start()
        {
            PrepareDeckForNewSeason();
        }

        void PrepareDeckForNewSeason()
        {
            ClearCardStack();
            if (gameOver)
            {
                return;
            }
            var unusedAmbushCards = seasonDeck.Where(x => x.availableTerrains.Any() && x.availableTerrains.All(t => t == CellType.Monster)).ToList();
            seasonDeck = new List<DiscoveryCard>(deck);
            seasonDeck.AddRange(unusedAmbushCards);
            AddAmbushCard();
            ShuffleSeasonDeck();
            // Tasowanie kart
        }

        private void AddAmbushCard()
        {
            if (ambushDeck.Count > 0)
            {
                var ambushCard = ambushDeck[Random.Range(0, ambushDeck.Count - 1)];
                seasonDeck.Add(ambushCard);
                ambushDeck.Remove(ambushCard);
            }
        }

        private void ShuffleSeasonDeck()
        {
            seasonDeck = seasonDeck.OrderBy(c => Random.value).ToList();
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
            PrepareDeckForNewSeason();
            if (gameOver)
            {
                BlockDrawButton();
                return;
            }
        }
    }
}
