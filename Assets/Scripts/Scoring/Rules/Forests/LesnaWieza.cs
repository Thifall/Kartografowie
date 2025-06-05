using Kartografowie.Grid;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Forests
{
    public class LesnaWieza : ForestsScoringRule
    {
        private const string RULE_NAME = "Leśna Wieża";
        private const string RULE_DESCRIPTION = "Zdobądź 1 punkt za każdy obszar lasu otoczony ze wszytskich czterech stron wypełnionymi obszarami lub krawędzią mapy";

        public LesnaWieza() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var forestCells = gridManager.GetSquares(c => c.CellType == General.CellType.Forest);
            foreach (var cell in forestCells)
            {
                var gridPos = cell.Key;
                if (gridManager.IsSquareRestricted(gridPos + Vector2Int.up) &&
                    gridManager.IsSquareRestricted(gridPos + Vector2Int.down) &&
                    gridManager.IsSquareRestricted(gridPos + Vector2Int.left) &&
                    gridManager.IsSquareRestricted(gridPos + Vector2Int.right))
                {
                    points++;
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
