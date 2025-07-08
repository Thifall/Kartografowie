using Kartografowie.General;
using Kartografowie.Shapes;
using Kartografowie.Terrains;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kartografowie.Cards.UI
{

    public class CardUI : MonoBehaviour
    {
        public CardDrawEventSO CardDrawEvent;
        public ShapeSelectedEventSO ShapeSelectedEvent;
        public TerrainSelectedEventSO TerrainSelectedEvent;
        public TextMeshProUGUI CardNameTextBox;
        public TextMeshProUGUI SeasonTimeValueTextBox;
        public Image BackgroundImage;
        public GameObject bonusShapeCoin;
        public Button[] terrainButtons;
        public Button[] shapeButtons;

        private float coinSpinAnimationCooldown = 2f;
        private float timeSinceLastCoinSpin = 0f;

        private void Update()
        {
            if (bonusShapeCoin != null && bonusShapeCoin.activeInHierarchy)
            {
                timeSinceLastCoinSpin += Time.deltaTime;
                if (timeSinceLastCoinSpin >= coinSpinAnimationCooldown)
                {
                    timeSinceLastCoinSpin = 0f;
                    TriggerCoinSpin();
                }
            }
        }

        private void TriggerCoinSpin()
        {
            var animator = bonusShapeCoin.GetComponent<Animator>();
            animator?.SetTrigger("triggerSpin");
        }

        public void SetupCardUI(DiscoveryCard card)
        {
            LoadCard(card);
        }

        private void LoadCard(DiscoveryCard card)
        {
            CardNameTextBox.text = card.CardName;
            SeasonTimeValueTextBox.text = card.TimeValue.ToString();
            BackgroundImage.sprite = card.BackgroundImage;
            if (!card.IsRuins)
            {
                SetTerains(card.availableTerrains);
                SetShapeIcons(card.ShapeIcons);
            }
            if (card.bonusShape)
            {
                if (bonusShapeCoin != null)
                {
                    bonusShapeCoin?.SetActive(true);
                }
            }
            else
            {
                if (bonusShapeCoin != null)
                {
                    bonusShapeCoin?.SetActive(false);
                }
            }
        }


        private void SetTerains(CellType[] availableTerrains)
        {
            foreach (var btn in terrainButtons)
            {
                btn.gameObject.SetActive(false);
                btn.onClick.RemoveAllListeners();
            }

            foreach (var terrain in availableTerrains)
            {
                var btn = terrainButtons.FirstOrDefault(b => b.name.Contains(terrain.ToString()));
                if (btn != null)
                {
                    btn.gameObject.SetActive(true);
                    btn.onClick.AddListener(() =>
                    {
                        SelectTerrain(terrain);
                    });
                }
            }
        }

        private void SetShapeIcons(Sprite[] shapeIcons)
        {
            for (int i = 0; i < shapeButtons.Length; i++)
            {
                if (i < shapeIcons.Length)
                {
                    shapeButtons[i].image.sprite = shapeIcons[i];
                    shapeButtons[i].gameObject.SetActive(true);
                    shapeButtons[i].GetComponentInParent<Transform>().gameObject.SetActive(true);

                    int index = i;
                    shapeButtons[i].onClick.RemoveAllListeners();
                    shapeButtons[i].onClick.AddListener(() =>
                    {
                        SelectShape(shapeIcons[index]);
                    });
                }
                else
                {
                    shapeButtons[i].gameObject.SetActive(false);
                    shapeButtons[i].GetComponentInParent<Transform>().gameObject.SetActive(false);
                }
            }
        }

        private void SelectTerrain(CellType cellType)
        {
            TerrainSelectedEvent.RaiseEvent(cellType);
        }

        private void SelectShape(Sprite shapeIcon)
        {
            ShapeSelectedEvent.RaiseEvent(shapeIcon);
        }

    }
}
