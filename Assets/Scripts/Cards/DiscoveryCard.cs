using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/New Card")]
public class DiscoveryCard : ScriptableObject
{
    public string CardName;
    public int TimeValue;
    public GameObject[] availableShapes; // Prefaby dostêpnych kszta³tów
    public Sprite[] ShapeIcons;
    public CellType[] availableTerrains; // Dostêpne tereny do wyboru
    public bool bonusShape;
    public Sprite BackgroundImage;

}
