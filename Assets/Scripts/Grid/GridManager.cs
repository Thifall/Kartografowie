using Kartografowie.General;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Grid
{
    public class GridManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2, GridCell> gridCells = new();
        private const float GRID_CELL_SIZE = 1.05f;
        private Vector3 gridOrigin = new(1, -5, 0);

        void Start()
        {
            GridCell[] allCells = FindObjectsByType<GridCell>(FindObjectsSortMode.None);
            foreach (GridCell cell in allCells)
            {
                Vector2 pos = WorldToGrid(cell.transform.position);
                gridCells[pos] = cell;
            }
        }

        public bool IsCellRestricted(Vector3 worldPos)
        {
            Vector2 gridPos = WorldToGrid(worldPos);
            return gridCells.ContainsKey(gridPos) && gridCells[gridPos].IsRestricted();
        }

        private Vector2 WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / GRID_CELL_SIZE);
            int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / GRID_CELL_SIZE);
            return new Vector2(x, y);
        }

        public void PaintCellAt(Vector2 position, CellType targetCellType)
        {
            gridCells[position].SetCellType(targetCellType);
        }

        public bool HasRuinsAtPosition(Vector3 worldPos)
        {
            Vector2 gridPos = WorldToGrid(worldPos);
            return gridCells[gridPos].HasRuins;
        }

        public IEnumerable<GridCell> GetAvailableEmptySquares(bool requiresRuins = false)
        {
            return gridCells.Values.Where(gc => gc.HasRuins == requiresRuins && gc.CellType == CellType.Default);
        }

        public bool CanDrawOnSquares(IEnumerable<Vector3> traversedAndOffsettedPositions)
        {
            return traversedAndOffsettedPositions
            .All((v) =>
            {
                Vector2 key = new(Mathf.Round(v.x / GRID_CELL_SIZE), Mathf.Round(v.y / GRID_CELL_SIZE));
                return gridCells.ContainsKey(key) && gridCells[key].CellType == CellType.Default;
            });
        }

        public string GetCellNameAtPosition(Vector3 pos)
        {
            return gridCells[new Vector2(Mathf.Round(pos.x / GRID_CELL_SIZE), Mathf.Round(pos.y / GRID_CELL_SIZE))].name;
        }
    } 
}
