using Kartografowie.Assets.Scripts.Grid.Runtime;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Forests
{
    public class Zagajnik : ForestsScoringRule
    {
        const string RULE_NAME = "Zagajnik";
        const string RULE_DESCRIPTION = @"Zdobądź 1 punkt za każdy rząd i każdą kolumnę z co najmniej jednym obszarem lasu.
                                    Ten sam obszar lasu może punktować zarówno w rzędzie, jak i kolumnie";

        public Zagajnik() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var (minX, maxX, minY, maxY) = gridManager.GetGridBounds();
            var points = 0;
            for (int i = minX; i <= maxX; i++)
            {
                if (gridManager.GetSquaresInColumn(i).Any(c => c.CurrentCellType == General.CellType.Forest))
                {
                    points++;
                }
            }
            for (int i = minY; i <= maxY; i++)
            {
                if (gridManager.GetSquaresInRow(i).Any(c => c.CurrentCellType == General.CellType.Forest))
                {
                    points++;
                }
            }
            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
