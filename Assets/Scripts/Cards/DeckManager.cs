using Kartografowie.Cards.UI;
using Kartografowie.General;
using Kartografowie.Shapes;
using Kartografowie.Terrains;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kartografowie.Cards
{
    public class DeckManager : MonoBehaviour
    {
        public List<DiscoveryCard> deck; // Lista wszystkich kart odkry�, punkt wyj�cia sezonu
        public List<AmbushCard> ambushDeck; // List kart zasadzki
        public SeasonManager seasonManager;
        public CardDrawEventSO cardDrawEvent;
        public ShapeSelectedEventSO shapeSelectedEvent;
        public TerrainSelectedEventSO terrainSelectedEvent;
        public SeasonEndEventSO seasonEndEvent;
        public GameObject NormalCardPrefab;
        public GameObject RuinsCardPrefab;
        public Transform CardStackParent;

        private List<DiscoveryCard> seasonDeck = new(); //u�ywany do manipulacji tali� podczas rozgrywki
        private readonly List<GameObject> activeCards = new();
        private bool gameOver = false;
        private DiscoveryCard currentCard;

        void Start()
        {
            PrepareDeckForNewSeason();
            seasonEndEvent.OnSeasonEnd += OnSeasonEnd;
        }

        void PrepareDeckForNewSeason()
        {
            if (gameOver)
            {
                return;
            }
            var unusedAmbushCards = seasonDeck.Where(x => x.availableTerrains.Any() && x.availableTerrains.All(t => t == CellType.Monster));
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
            gameObject.GetComponent<Button>().interactable = false;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Finished!";
        }

        public void OnSeasonEnd(Seasons endingSeason, bool gameEnded)
        {
            ClearCardStack();
            gameOver = gameEnded;
            if (gameOver)
            {
                BlockDrawButton();
                return;
            }
            PrepareDeckForNewSeason();
        }
    }
}
