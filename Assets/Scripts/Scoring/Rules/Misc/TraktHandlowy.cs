using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.General;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Misc
{
    public class TraktHandlowy : MiscScoringRule
    {
        private const string RULE_NAME = "Trakt Handlowy";
        private const string RULE_DESCRIPTION = "Zdobądź 3 punkty za każdą kompletnąukośną linię z wypełnionych obszarów, która łączy lewą i dolną krawędź mapy";

        public TraktHandlowy() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;
            var direction = Vector2Int.down + Vector2Int.right;
            var (minX, _, minY, maxY) = gridManager.GetGridBounds();

            for (int i = maxY; i >= minY; i--)
            {
                if (IsLineFilled(gridManager, new Vector2Int(minX, i), direction))
                {
                    points += 3;
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }

        private bool IsLineFilled(GridManager gridManager, Vector2Int currentPosition, Vector2Int direction)
        {
            var currentSquare = gridManager.GetSquareByPosition(currentPosition);
            if (currentSquare == null) //out of bounds = positive check
            {
                return true;
            }

            if (currentSquare.CurrentCellType == CellType.Default)
            {
                return false;
            }

            return IsLineFilled(gridManager, currentPosition + direction, direction);
        }
    }
}
