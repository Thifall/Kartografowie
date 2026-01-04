using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Monsters
{
    public class MonsterPenaltyRule : ScoringRule
    {
        private const string _ruleName = "Monster Penalty";
        private const string _ruleDescription = "Deducts 1 point from score, for each empty square that surrounds square of monster type";

        public MonsterPenaltyRule() : base(_ruleName, _ruleDescription)
        {
        }

        public override Edicts EdictType => Edicts.Monesters;

        public override RuleType RuleType => RuleType.Monsters;

        public override int CalculateScore(GridManager gridManager)
        {
            var monsterSquares = gridManager.GetSquares(c => c.CurrentCellType == CellType.Monster);
            var visited = new HashSet<Vector2Int>();
            foreach (var square in monsterSquares)
            {
                var cell = square.Value;
                foreach (var neighbor in Generals.Directions.Select(d => cell.GridPosition + d))
                {
                    var neighborCell = gridManager.GetSquareByPosition(neighbor);
                    if (neighborCell != null && neighborCell.CurrentCellType == CellType.Default)
                    {
                        visited.Add(neighbor);
                    }
                }
            }
            return -visited.Count;
        }
    }
}
