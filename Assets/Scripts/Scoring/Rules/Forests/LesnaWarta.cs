using Kartografowie.Grid;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Forests
{
    public class LesnaWarta : ForestsScoringRule
    {
        private const string RULE_NAME = "Leśna Warta";
        private const string DESCRIPTION = @"Zdobądź 1 punkt za każdy obszar lasu sąsiadujący z krawędzią mapy";

        public LesnaWarta() : base(RULE_NAME, DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var bounds = gridManager.GetGridBounds();
            int points = 0;

            points += gridManager.GetSquares((c) => c.CellType == General.CellType.Forest &&
            (
             c.GridPosition.x == bounds.minX || c.GridPosition.x == bounds.maxX
             || c.GridPosition.y == bounds.minY || c.GridPosition.y == bounds.maxY
            )).Count;

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
