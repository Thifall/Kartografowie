using Kartografowie.General;
using Kartografowie.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public class ZyzneRowniny : VilageScoringRule
    {
        private const string RULE_NAME = "Żyzne Równiny";
        private const string RULE_DESCRIPTION = @"Zdobądź 3 punkty za każdy klaster obszarów wioski, który sąsiaduje z 3 lub więcej różnymi rodzajami terenu";

        public ZyzneRowniny() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var clusters = GetClusters(gridManager, CellType.Village);

            foreach (var cluster in clusters)
            {
                var distinctNeighbours = new HashSet<CellType>();
                foreach (var cell in cluster)
                {
                    foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                    {
                        var neighbor = gridManager.GetSquareByPosition(cell.GridPosition + direction);
                        if (neighbor != null)
                        {
                            Debug.Log($"Cluster contains square: {neighbor} at {neighbor.GridPosition}: {cluster.Contains(neighbor)}");
                        }
                        if (neighbor != null
                            && !cluster.Contains(neighbor)
                            && neighbor.CellType != CellType.Default
                            && neighbor.CellType != CellType.Chasm) 
                        {
                            distinctNeighbours.Add(neighbor.CellType);
                            Debug.Log($"Found non-vilage filled square: {neighbor.CellType} at {neighbor.GridPosition}:");
                        }
                    }
                }
                if (distinctNeighbours.Count >= 3)
                {
                    Debug.Log($"Cluster at {cluster.First().GridPosition} has 3 or more neighbors of different types. Awarding points");
                    points += 3;
                }
                else
                {
                    Debug.Log($"Cluster at {cluster.First().GridPosition} has not got enough neighbors of different types. No points");
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
