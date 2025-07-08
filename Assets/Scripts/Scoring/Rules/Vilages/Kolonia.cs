using Kartografowie.General;
using Kartografowie.Grid;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public class Kolonia : VilageScoringRule
    {
        private const string RULE_NAME = "Kolonia";
        private const string RULE_DESCRIPTION = @"Zdobądź 8 punktów za każdy klaster składający się z 6 lub więcej obszarów wioski";

        public Kolonia() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var villageClusters = GetClusters(gridManager, CellType.Village);
            points += villageClusters
                .Where(cluster => cluster.Count >= 6) // Sprawdzamy, czy klaster ma co najmniej 6 obszarów wioski
                .Count() * 8; // Każdy taki klaster daje 8 punktów


            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
