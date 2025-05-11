using Kartografowie.General;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Grid
{
    public class GridManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2Int, GridCell> gridCells = new();
        private const float GRID_CELL_SIZE = 1.05f;
        private Vector3 gridOriginTraverse = Vector3.zero;

        void Start()
        {
            AssignGridOriginTraverse();
            GridCell[] allCells = FindObjectsByType<GridCell>(FindObjectsSortMode.None);
            foreach (GridCell cell in allCells)
            {
                Vector2Int gridPos = WorldToGrid(cell.transform.position);
                gridCells[gridPos] = cell;
            }
        }

        public bool IsCellRestricted(Vector3 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);
            return gridCells.ContainsKey(gridPos) && gridCells[gridPos].IsRestricted();
        }

        public void PaintCellAt(Vector3 position, CellType targetCellType)
        {
            var pos = WorldToGrid(position);
            gridCells[pos].SetCellType(targetCellType);
        }

        public bool HasRuinsAtPosition(Vector3 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);
            return gridCells[gridPos].HasRuins;
        }

        public IEnumerable<GridCell> GetAvailableEmptySquares(bool requiresRuins = false)
        {
            return gridCells.Values.Where(gc => gc.HasRuins == requiresRuins && gc.CellType == CellType.Default);
        }

        public bool CanDrawOnSquares(IList<Vector3> traversedAndOffsettedPositions)
        {
            return traversedAndOffsettedPositions
            .All((v) =>
            {
                Vector2Int key = new(Mathf.RoundToInt(v.x / GRID_CELL_SIZE), Mathf.RoundToInt(v.y / GRID_CELL_SIZE));
                return gridCells.ContainsKey(key) && gridCells[key].CellType == CellType.Default;
            });
        }

        public string GetCellNameAtPosition(Vector3 pos)
        {
            return gridCells[new Vector2Int(Mathf.RoundToInt(pos.x / GRID_CELL_SIZE), Mathf.RoundToInt(pos.y / GRID_CELL_SIZE))].name;
        }

        public float GetCellSize()
        {
            return GRID_CELL_SIZE;
        }

        public Vector3 GetGridOriginTraverse()
        {
            return gridOriginTraverse;
        }

        public bool IsWithinGridBounds(Vector3 position)
        {

            return gridCells.ContainsKey(WorldToGrid(position));
        }

        private void AssignGridOriginTraverse()
        {
            var gridLayoutPlaceholder = FindFirstObjectByType<LayoutSetup>();
            if (gridLayoutPlaceholder != null)
            {
                gridOriginTraverse = gridLayoutPlaceholder.transform.position;
            }
        }

        private Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 offset = worldPos - gridOriginTraverse;
            float epsilon = 0.00001f;
            int x = Mathf.FloorToInt((offset.x + epsilon) / GRID_CELL_SIZE);
            int y = Mathf.FloorToInt((offset.y + epsilon) / GRID_CELL_SIZE);
            return new Vector2Int(x, y);
        }

        public Vector3 GetCellPositionFromCursorPosition()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var adjustedTraverse = gridOriginTraverse - new Vector3(GRID_CELL_SIZE / 2f, GRID_CELL_SIZE / 2f, 0f);

            float localX = mousePos.x - adjustedTraverse.x;
            float localY = mousePos.y - adjustedTraverse.y;

            int cellX = Mathf.FloorToInt(localX / GRID_CELL_SIZE);
            int cellY = Mathf.FloorToInt(localY / GRID_CELL_SIZE);

            float snappedX = cellX * GRID_CELL_SIZE + gridOriginTraverse.x;
            float snappedY = cellY * GRID_CELL_SIZE + gridOriginTraverse.y;

            return new Vector3(snappedX, snappedY, 0f);
        }
    }
}
