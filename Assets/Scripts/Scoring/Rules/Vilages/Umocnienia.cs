using Kartografowie.General;
using Kartografowie.Grid;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public class Umocnienia : VilageScoringRule
    {
        private const string RULE_NAME = "Umocnienia";
        private const string RULE_DESCRIPTION = "Zdobądź 2 punkty za każdy obszar wioski w drugim co do wielkości klastrze obszarów wioski.";
        public Umocnienia() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;
            var clusters = GetClusters(gridManager, CellType.Vilage);

            var gruppedClusters = clusters.GroupBy(cluster => cluster.Count).
                OrderByDescending(g => g.Key).ToList();

            if(gruppedClusters.Count >= 2)
            {
                var secondLargest = gruppedClusters[1];
                points += secondLargest.Sum(cluster => cluster.Count() * 2);
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
