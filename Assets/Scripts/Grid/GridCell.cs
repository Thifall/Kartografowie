using Kartografowie.General;
using UnityEngine;

namespace Kartografowie.Grid
{
    public class GridCell : MonoBehaviour
    {
        public CellType CellType { get; private set;} = CellType.Default;
        public bool HasRuins = false;
        public Vector2Int GridPosition = Vector2Int.zero;

        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnValidate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateVisual(); // Aktualizuje kolor po zmianie wartoœci w Inspectorze
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null) return;

            spriteRenderer.color = Generals.CellTypeColors[CellType];
        }

        public bool IsRestricted()
        {
            return CellType != CellType.Default;
        }

        public void SetCellType(CellType newType)
        {
            CellType = newType;
            UpdateVisual();
        }
    }
}