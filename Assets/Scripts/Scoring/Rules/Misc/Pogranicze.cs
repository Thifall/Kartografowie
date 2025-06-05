using Kartografowie.Grid;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Misc
{
    public class Pogranicze : MiscScoringRule
    {
        private const string RULE_NAME = "Pogranicze";
        private const string RULE_DESCRIPTION = "Zdobądź 6 punktów za każdy kompletny rząd lub kolumnę wypełnionych obszarów";

        public Pogranicze() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var (minX, maxX, minY, maxY) = gridManager.GetGridBounds();

            for (var i = minX; i <= maxX; i++)
            {
                var column = gridManager.GetSquaresInColumn(i);
                if(!column.Any(c => c.CellType == default))
                {
                    points += 6;
                }

                var row = gridManager.GetSquaresInRow(i);
                if(!row.Any(c => c.CellType == default))
                {
                    points += 6;
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
