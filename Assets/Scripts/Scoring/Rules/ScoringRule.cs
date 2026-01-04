using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.General;
using System.Collections.Generic;
using UnityEngine;


namespace Kartografowie.Assets.Scripts.Scoring.Rules
{
    public abstract class ScoringRule
    {
        private readonly string RuleName;
        private readonly string RuleDescription;
        public abstract Edicts EdictType { get; }

        public ScoringRule(string ruleName, string ruleDescription)
        {
            RuleName = ruleName;
            RuleDescription = ruleDescription;
        }

        public abstract RuleType RuleType { get; }

        public abstract int CalculateScore(GridManager gridManager);

        public string GetDescription()
        {
            return RuleDescription;
        }
        public string GetName()
        {
            return RuleName;
        }

        protected List<List<GridCell>> GetClusters(GridManager gridManager, CellType cellType)
        {
            var typedSquares = gridManager.GetSquares(c => c.CurrentCellType == cellType);
            var visited = new HashSet<Vector2Int>();
            var clusters = new List<List<GridCell>>();
            foreach (var kv in typedSquares)
            {
                var startPosition = kv.Key;

                if (visited.Contains(startPosition)) continue;
                var squaresToVisit = new Queue<Vector2Int>();

                var cluster = new List<GridCell>();
                squaresToVisit.Enqueue(startPosition);
                while (squaresToVisit.Count > 0)
                {
                    var currentSquare = squaresToVisit.Dequeue();
                    if (visited.Contains(currentSquare)) continue;
                    cluster.Add(typedSquares[currentSquare]);
                    visited.Add(currentSquare);
                    // Sprawdzamy sąsiednie kwadraty
                    foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                    {
                        var neighbor = currentSquare + direction;
                        if (typedSquares.ContainsKey(neighbor) && !visited.Contains(neighbor))
                        {
                            squaresToVisit.Enqueue(neighbor);
                        }
                    }
                }
                clusters.Add(cluster);
                Debug.Log($"Znaleziono klaster {cellType} o rozmiarze {cluster.Count} w pozycji {startPosition}");
            }
            return clusters;
        }
    }

    public enum RuleType
    {
        Misc,
        Forests,
        Vilages,
        PlainsAndWaters,
        Monsters,
        Coins
    }
}
