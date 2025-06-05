using Kartografowie.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Misc
{
    public class Kryjowki : MiscScoringRule
    {
        private const string RULE_NAME = "Kryjówki";
        private const string RULE_DESCRIPTION = @"Zdobądź  1 punkt za każdy pusty obszar otoczony ze wszystkin 4 stron wypełnionymi obszarami lub krawędzią mapy";


        public Kryjowki() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            Dictionary<Vector2Int, GridCell> emptySquares = gridManager.GetAvailableEmptySquares().ToDictionary(c => c.GridPosition);
            foreach (var kv in emptySquares)
            {
                GridCell currentSquare = kv.Value;
                var hasEmptyNeighbour = false;
                foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    if (emptySquares.ContainsKey(currentSquare.GridPosition + direction))
                    {
                        hasEmptyNeighbour = true;
                    }
                }
                points += hasEmptyNeighbour ? 0 : 1;
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
