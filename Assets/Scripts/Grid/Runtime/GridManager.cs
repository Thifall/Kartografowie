using Kartografowie.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Grid.Runtime
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
                cell.GridPosition = gridPos;
            }
        }

        public bool IsSquareRestricted(Vector3 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);
            return IsSquareRestricted(gridPos); //gridCells.ContainsKey(gridPos) && gridCells[gridPos].IsRestricted();
        }

        public bool IsSquareRestricted(Vector2Int gridPos)
        {
            if (!gridCells.ContainsKey(gridPos))
            {
                return true; 
            }
            return gridCells[gridPos].IsRestricted();
        }

        public IEnumerator PaintShapeAtWorldPos(IEnumerable<Vector3> positions, CellType targetCellType)
        {
            foreach (var pos in positions)
            {
                PaintSquareAtWorldPos(pos, targetCellType);
                yield return new WaitForSeconds(.25f);
            }
        }

        public void PaintSquareAtWorldPos(Vector3 position, CellType targetCellType)
        {
            var pos = WorldToGrid(position);
            gridCells[pos].SetCellType(targetCellType);
        }

        public void PaintSquareAtGridPos(Vector2Int position, CellType targetCellType)
        {
            gridCells[position].SetCellType(targetCellType);
        }

        public bool HasRuinsAtPosition(Vector3 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);
            return gridCells[gridPos].HasRuins;
        }

        public IEnumerable<GridCell> GetAvailableEmptySquares(bool requiresRuins = false)
        {
            return gridCells.Values.Where(gc => gc.HasRuins == requiresRuins && gc.CurrentCellType == CellType.Default);
        }

        public bool CanDrawOnSquares(IList<Vector3> traversedAndOffsettedPositions)
        {
            return traversedAndOffsettedPositions
            .All((v) =>
            {
                Vector2Int key = new(Mathf.RoundToInt(v.x / GRID_CELL_SIZE), Mathf.RoundToInt(v.y / GRID_CELL_SIZE));
                return gridCells.ContainsKey(key) && gridCells[key].CurrentCellType == CellType.Default;
            });
        }

        public string GetSquareNameAtPosition(Vector3 pos)
        {
            return gridCells[new Vector2Int(Mathf.RoundToInt(pos.x / GRID_CELL_SIZE), Mathf.RoundToInt(pos.y / GRID_CELL_SIZE))].name;
        }

        public GridCell GetSquareByPosition(Vector2Int position)
        {
            return gridCells.GetValueOrDefault(position);
        }

        public float GetSquareSize()
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

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 offset = worldPos - gridOriginTraverse;
            return PositionToGrid(offset);
        }

        public Vector2Int PositionToGrid(Vector3 position)
        {
            float epsilon = 0.00001f;
            int x = Mathf.FloorToInt((position.x + epsilon) / GRID_CELL_SIZE);
            int y = Mathf.FloorToInt((position.y + epsilon) / GRID_CELL_SIZE);
            return new Vector2Int(x, y);
        }

        public Vector3 GetSquarePositionFromCursorPosition()
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

        public List<GridCell> GetSquaresInLine(IEnumerable<GridCell> cells, Vector2Int direction, Vector2Int corner)
        {
            // Filtr — zale¿nie od kierunku porównujemy albo oœ Y (dla poziomego ruchu), albo X (dla pionowego)
            Func<GridCell, bool> filter = direction.x != 0
                ? c => c.GridPosition.y == corner.y
                : c => c.GridPosition.x == corner.x;

            // Klucz sortuj¹cy — zale¿nie od kierunku porz¹dkujemy rosn¹co lub malej¹co
            Func<GridCell, int> keySelector = direction.x != 0
                ? direction.x > 0 ?
                    new Func<GridCell, int>(c => c.GridPosition.x) :
                    new Func<GridCell, int>(c => -c.GridPosition.x)
                : direction.y > 0 ?
                    new Func<GridCell, int>(c => c.GridPosition.y) :
                    new Func<GridCell, int>(c => -c.GridPosition.y);

            return cells.Where(filter).OrderBy(keySelector).ToList();
        }

        public (int minX, int maxX, int minY, int maxY) GetGridBounds()
        {
            if (gridCells.Count == 0)
            {
                return (0, 0, 0, 0);
            }
            int minX = gridCells.Keys.Min(k => k.x);
            int maxX = gridCells.Keys.Max(k => k.x);
            int minY = gridCells.Keys.Min(k => k.y);
            int maxY = gridCells.Keys.Max(k => k.y);
            return (minX, maxX, minY, maxY);
        }

        public List<GridCell> GetSquaresInRow(int rowIndex)
        {
            return gridCells.Values
                .Where(cell => cell.GridPosition.y == rowIndex)
                .OrderBy(cell => cell.GridPosition.x)
                .ToList();
        }

        public List<GridCell> GetSquaresInColumn(int columnIndex)
        {
            return gridCells.Values
                .Where(cell => cell.GridPosition.x == columnIndex)
                .OrderBy(cell => cell.GridPosition.y)
                .ToList();
        }

        public Dictionary<Vector2Int,GridCell> GetSquares(Func<GridCell, bool> filter)
        {
            return gridCells.Values.Where(filter).Select(c => new KeyValuePair<Vector2Int, GridCell>(c.GridPosition, c))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private void AssignGridOriginTraverse()
        {
            var gridLayoutPlaceholder = FindFirstObjectByType<LayoutSetup>();
            if (gridLayoutPlaceholder != null)
            {
                gridOriginTraverse = gridLayoutPlaceholder.transform.position;
            }
        }
    }
}
