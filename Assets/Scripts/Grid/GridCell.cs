using Kartografowie.General;
using UnityEngine;

namespace Kartografowie.Grid
{
    public class GridCell : MonoBehaviour
    {
        public CellType cellType = CellType.Default;
        public bool HasRuins = false;

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

            spriteRenderer.color = Generals.CellTypeColors[cellType];
        }

        internal bool IsRestricted()
        {
            return cellType != CellType.Default;
        }

        public void SetCellType(CellType newType)
        {
            cellType = newType;
            UpdateVisual();
        }
    }
}