using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.General;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Forests
{
    public class GorskieOstepy : ForestsScoringRule
    {
        private const string _ruleName = "Górskie Ostępy";
        private const string _ruleDescription = "Zdobądź 3 punkty za każdy obszar gór, połączony z innym obszarem gór przez klaster lasu";

        public GorskieOstepy() : base(_ruleName, _ruleDescription)
        {
        }
        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var forestSquares = gridManager.GetSquares(c => c.CurrentCellType == CellType.Forest);
            var mountainsquares = gridManager.GetSquares(c => c.CurrentCellType == CellType.Mountain);

            var visited = new HashSet<Vector2Int>();

            foreach (var square in forestSquares)
            {
                var startPosition = square.Key;

                if (visited.Contains(startPosition)) continue; // Jeśli już odwiedziliśmy ten kwadrat, pomijamy go

                var forestCluster = new List<Vector2Int>();
                var mountainsTilesTouching = 0;
                var squarestoVisit = new Queue<Vector2Int>();
                var mountainTilesVisited = new HashSet<Vector2Int>();

                squarestoVisit.Enqueue(startPosition);

                while (squarestoVisit.Count > 0)
                {
                    var currentSquare = squarestoVisit.Dequeue();
                    if (visited.Contains(currentSquare)) continue; // Jeśli już odwiedziliśmy ten kwadrat, pomijamy go

                    if (!forestSquares.ContainsKey(currentSquare)) continue; //pomiń, jeśli nie jest lasem

                    visited.Add(currentSquare);
                    forestCluster.Add(currentSquare);
                    // Sprawdzamy sąsiednie kwadraty
                    foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                    {
                        var neighbor = currentSquare + direction;
                        if (forestSquares.ContainsKey(neighbor) && !visited.Contains(neighbor))
                        {
                            squarestoVisit.Enqueue(neighbor);
                        }

                        if (mountainsquares.ContainsKey(neighbor) && !mountainTilesVisited.Contains(neighbor))
                        {
                            mountainsTilesTouching++;
                            mountainTilesVisited.Add(neighbor);
                        }
                    }
                }

                if (forestCluster.Any() && mountainsTilesTouching >= 2)
                {
                    points += 3 * mountainsTilesTouching;
                    foreach (var mountainTile in mountainTilesVisited)
                    {
                        mountainsquares.Remove(mountainTile); // Usuwamy odwiedzone kwadraty górskie, aby nie liczyć ich ponownie
                    }
                }
            }

            Debug.Log($"Punkty za regułę \"{_ruleName}\": {points}");
            return points;
        }
    }
}
