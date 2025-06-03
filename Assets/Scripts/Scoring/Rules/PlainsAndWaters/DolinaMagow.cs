using Kartografowie.General;
using Kartografowie.Grid;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters
{
    public class DolinaMagow : PlainsAndWatersScoringRule
    {
        private const string RULE_NAME = "Dolina Magów";
        private const string RULE_DESCRIPTION = @"Zdobądź 2 punkty za każdy obszar wody sąsiadujący z obszarem gór.
                                                  Zdobądź 1 punkt za każdy obszar pola sąsiadujący z obszarem gór.";
        public DolinaMagow() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var mountainSquares = gridManager.GetCells(c => c.CellType == CellType.Mountain);
            var waterSquares = gridManager.GetCells(c => c.CellType == CellType.Water);
            var fieldSquares = gridManager.GetCells(c => c.CellType == CellType.Field);

            foreach (var kv in mountainSquares)
            {
                var mountain = kv.Value;
                foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var neighbor = mountain.GridPosition + direction;
                    if (waterSquares.ContainsKey(neighbor))
                    {
                        points += 2; // 2 points for water adjacent to mountain
                    }
                    else if (fieldSquares.ContainsKey(neighbor))
                    {
                        points += 1; // 1 point for field adjacent to mountain
                    }
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
