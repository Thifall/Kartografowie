using Kartografowie.General;
using Kartografowie.Grid;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public class WielkieMiasto : VilageScoringRule
    {
        private const string RULE_NAME = "Wielkie Miasto";
        private const string RULE_DESCRIPTION = "Zdobądź 1 punkt za każdy obszar wioski w największym klastzre wiosek, który nie sąsiaduje z obszarem gór";
        public WielkieMiasto() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var clusters = GetClusters(gridManager, CellType.Vilage).OrderByDescending(c => c.Count).ToList(); //wszystkie klastry od największego do najmniejszego
            var mountainSquares = gridManager.GetSquares(c => c.CellType == CellType.Mountain);


            if (clusters.Count() == 0)
            {
                Debug.Log("No clusters found for scoring.");
                return points;
            }

            var found = false;
            while (!found && clusters.Count() > 0)
            {
                var currentCluster = clusters.First();
                var touchesMountain = currentCluster.Any(c =>
                    mountainSquares.ContainsKey(c.GridPosition + Vector2Int.up) ||
                    mountainSquares.ContainsKey(c.GridPosition + Vector2Int.down) ||
                    mountainSquares.ContainsKey(c.GridPosition + Vector2Int.left) ||
                    mountainSquares.ContainsKey(c.GridPosition + Vector2Int.right));

                if (touchesMountain)
                {
                    Debug.Log($"Cluster at {currentCluster.First().GridPosition} is adjacent to mountains, skipping scoring.");
                }
                else {                     // Dodaj punkty za ten klaster
                    points += currentCluster.Count;
                    found = true;
                    Debug.Log($"Scoring cluster at {currentCluster.First().GridPosition} with {currentCluster.Count} points.");
                }

                clusters.RemoveAt(0); //usuwamy sprawdzony klaster
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
