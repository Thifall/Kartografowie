using Kartografowie.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Grid
{
    public class GridManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2, GridCell> gridCells = new();
        private const float GRID_CELL_SIZE = 1.05f;
        private Vector3 gridOriginTraverse = Vector3.zero;

        void Start()
        {
            AssignGridOriginTraverse();
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

        public void PaintCellAt(Vector3 position, CellType targetCellType)
        {
            var pos = WorldToGrid(position);
            gridCells[pos].SetCellType(targetCellType);
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

        private Vector2 WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x - gridOriginTraverse.x / GRID_CELL_SIZE);
            int y = Mathf.RoundToInt(worldPos.y - gridOriginTraverse.y / GRID_CELL_SIZE);
            return new Vector2(x, y);
        }

        public Vector3 GetCellPositionFromCursorPosition()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //prze³o¿enie pozycji na wspó³rzêdne x/y 
            Vector3 pos = new(Mathf.Round(mousePos.x / GRID_CELL_SIZE), Mathf.Round(mousePos.y / GRID_CELL_SIZE));

            //obliczenie wierszy/kolumn wzglêdem punktu rozpoczêcia siatki
            var offsett = gridOriginTraverse - pos;

            //Obliczanie pozycji, zak³adaj¹c, ¿e nie musi byæ ona na gridzie. 
            //Do zaokr¹glonej pozycji dodajemy ró¿nice miêdzy iloœci¹ komórek a ich faktycznym rozmiarem
            var position = new Vector3(pos.x + (offsett.x - offsett.x * GRID_CELL_SIZE), pos.y + (offsett.y - offsett.y * GRID_CELL_SIZE));
            return position;
        }
    }
}
