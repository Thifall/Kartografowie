using Kartografowie.General;
using Kartografowie.Shapes;
using Kartografowie.Terrains;
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
        public Button[] terrainButtons;
        public Button[] shapeButtons;

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
        }

        private void SetTerains(CellType[] availableTerrains)
        {
            for (int i = 0; i < terrainButtons.Length; i++)
            {
                if (i < availableTerrains.Length)
                {
                    terrainButtons[i].image.color = Generals.CellTypeColors[availableTerrains[i]];
                    terrainButtons[i].gameObject.SetActive(true);

                    int index = i;
                    terrainButtons[i].onClick.RemoveAllListeners();
                    terrainButtons[i].onClick.AddListener(() =>
                    {
                        SelectTerrain(availableTerrains[index]);
                    });
                }
                else
                {
                    terrainButtons[i].gameObject.SetActive(false);
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
