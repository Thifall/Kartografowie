using Kartografowie.Grid;
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
            var bounds = gridManager.GetGridBounds();
            var points = 0;
            for (int i = bounds.minX; i <= bounds.maxX; i++)
            {
                if (gridManager.GetCellsInColumn(i).Any(c => c.CellType == General.CellType.Forest))
                {
                    points++;
                }
            }
            for (int i = bounds.minY; i <= bounds.maxY; i++)
            {
                if (gridManager.GetCellsInRow(i).Any(c => c.CellType == General.CellType.Forest))
                {
                    points++;
                }
            }
            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
