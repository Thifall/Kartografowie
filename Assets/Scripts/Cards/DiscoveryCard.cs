using Kartografowie.General;
using UnityEngine;

namespace Kartografowie.Cards
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/New Card")]
    public class DiscoveryCard : ScriptableObject
    {
        public string CardName;
        public int TimeValue;
        public GameObject[] availableShapes; // Prefaby dost�pnych kszta�t�w
        public Sprite[] ShapeIcons;
        public CellType[] availableTerrains; // Dost�pne tereny do wyboru
        public bool bonusShape;
        public Sprite BackgroundImage;
        public bool IsRuins = false;
    }
}