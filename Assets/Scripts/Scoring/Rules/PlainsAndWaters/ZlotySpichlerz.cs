using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.General;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters
{
    public class ZlotySpichlerz : PlainsAndWatersScoringRule
    {
        private const string RULE_NAME = "Złoty Spichlerz";
        private const string RULE_DESCRIPTION = @"Zdobądź 1 punkt za każdy obszar wody sąsiadujący z obszerem ruin
                                                  Zdobądź 3 punkty za każdy obszar pola na obszarze ruin";

        public ZlotySpichlerz() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var waters = gridManager.GetSquares(c => c.CurrentCellType == CellType.Water);
            var ruins = gridManager.GetSquares(c => c.HasRuins);

            points += 3 * ruins.Count(c => c.Value.CurrentCellType == CellType.Field);

            foreach (var kv in waters)
            {
                var waterCell = kv.Value;
                foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var neighbor = waterCell.GridPosition + direction;
                    if (ruins.ContainsKey(neighbor))
                    {
                        points += 1;
                    }
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
