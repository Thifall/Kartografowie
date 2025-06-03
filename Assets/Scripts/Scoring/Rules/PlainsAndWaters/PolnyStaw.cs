using Kartografowie.General;
using Kartografowie.Grid;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters
{
    public class PolnyStaw : PlainsAndWatersScoringRule
    {
        private const string RULE_NAME = "Polny Staw";
        private const string RULE_DESCRIPTION = @"Zdobądź 1 punkt za każdy obszar wody sąsiadujący z przynajmniej jednym obszerem pola.
                                                  Zdobądź 1 punkt za każdy obszar pola sąsiadujący z przynajmniej jednym obszarem wody.";

        public PolnyStaw() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            points += CountSquaresWithAtLeastOneNeighbour(CellType.Field, CellType.Water, gridManager);
            points += CountSquaresWithAtLeastOneNeighbour(CellType.Water, CellType.Field, gridManager);

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return 0;
        }

        private static int CountSquaresWithAtLeastOneNeighbour(CellType searchedType, CellType lookedForType, GridManager grid)
        {
            var points = 0;
            var searchedCells = grid.GetCells(c => c.CellType == searchedType);
            var lookedForCells = grid.GetCells(c => c.CellType == lookedForType);
            foreach (var kv in searchedCells)
            {
                var currentSquare = kv.Value;
                foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var neighbor = currentSquare.GridPosition + direction;
                    if (lookedForCells.ContainsKey(neighbor))
                    {
                        points++;
                        break;
                    }
                }
            }

            return points;
        }
    }
}
