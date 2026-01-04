using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.Assets.Scripts.Scoring.Events;
using Kartografowie.General;
using Kartografowie.Shapes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Core
{
    public class CoinTracker
    {
        private readonly OnCoinAddedEventSO _onCoinAddedEvent;
        private readonly GridManager _gridManager;
        private readonly HashSet<Vector2Int> _visitedMountains = new();

        public int CoinsCount { get; private set; }

        public CoinTracker(ShapeDrawnEventSO drawnEventSO, OnCoinAddedEventSO onCoinAddedEvent, GridManager gridManager)
        {
            drawnEventSO.OnShapeDrawn += OnShapeDrawn;
            _onCoinAddedEvent = onCoinAddedEvent;
            _gridManager = gridManager;
        }

        private void OnShapeDrawn(Shape shape)
        {
            if (shape != null && shape.IsBonusShape)
            {
                CoinsCount++;
                _onCoinAddedEvent.RaiseOnCoinAddedEvent();
            }
            CheckForSurroundedMountains();
        }

        private void CheckForSurroundedMountains()
        {
            var mountains = _gridManager
                .GetSquares(c => c.CurrentCellType == CellType.Mountain)
                .Select(c => c.Value)
                .ToList();

            foreach (var cell in mountains)
            {
                bool isSurrounded = true;
                foreach (var neighbor in Generals.Directions.Select(d => cell.GridPosition + d))
                {
                    var neighborCell = _gridManager.GetSquareByPosition(neighbor);
                    if (neighborCell != null && neighborCell.CurrentCellType == CellType.Default)
                    {
                        isSurrounded = false; // Found an empty neighbor, not surrounded
                        break;
                    }
                }
                if (isSurrounded && !_visitedMountains.Contains(cell.GridPosition))
                {
                    Debug.Log($"Mountain at {cell.GridPosition} is surrounded by other mountains.");
                    CoinsCount++;
                    _onCoinAddedEvent.RaiseOnCoinAddedEvent();
                    _visitedMountains.Add(cell.GridPosition); // Mark this mountain as visited
                }
            }
        }
    }
}
