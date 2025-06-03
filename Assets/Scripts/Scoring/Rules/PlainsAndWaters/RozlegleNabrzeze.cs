using Kartografowie.General;
using Kartografowie.Grid;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters
{
    public class RozlegleNabrzeze : PlainsAndWatersScoringRule
    {
        private const string RULE_NAME = "Rozległe Nabrzeże";
        private const string RULE_DESCRIPTION = @"Zdobądź 3 punkty za każdy klaster pola nie sąsiadujący z obszrem wody lub krawędzią mapy.
                                                  Zdobądź 3 punkt za każdy klaster wody nie sąsiadujący z obszarem pola lub krawędzią mapy.";

        public RozlegleNabrzeze() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;
            var (minX, maxX, minY, maxY) = gridManager.GetGridBounds();

            foreach (var cellType in new[] { CellType.Field, CellType.Water })
            {
                List<List<GridCell>> clusters = GetClusters(gridManager, cellType);
                foreach (var cluster in clusters)
                {
                    var unwantedCellType = cellType == CellType.Field ? CellType.Water : CellType.Field;
                    if (!IsClusterOnMapEdge(minX, maxX, minY, maxY, cluster) 
                        && !ClusterTouchesType(cluster, gridManager.GetCells(c => c.CellType ==  unwantedCellType)))
                    {
                        points += 3;
                    }
                    else
                    {
                        Debug.Log($"Klaster sąsiaduje z krawędzią mapy lub innym klastrem {unwantedCellType}, nie przyznajemy punktów.");
                    }
                }
            }

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }

        private bool ClusterTouchesType(List<GridCell> cluster, Dictionary<Vector2Int, GridCell> unwantedTypeSquares)
        {
            foreach(var cell in cluster)
            {
                var neighbors = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }
                    .Select(direction => cell.GridPosition + direction);
                foreach (var neighbor in neighbors)
                {
                    if(unwantedTypeSquares.ContainsKey(neighbor))
                    {
                        Debug.Log($"Klaster {cell.CellType} sąsiaduje z {unwantedTypeSquares[neighbor].CellType} w pozycji {neighbor}.");
                        return true; // Klaster dotyka innego typu
                    }
                }
            }
            return false; // Klaster nie dotyka innego typu
        }

        private List<List<GridCell>> GetClusters(GridManager gridManager, CellType cellType)
        {
            var typedSquares = gridManager.GetCells(c => c.CellType == cellType);
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

        private static bool IsClusterOnMapEdge(int minX, int maxX, int minY, int maxY, List<GridCell> cluster)
        {
            return cluster.Any(c => c.GridPosition.x == minX
                                || c.GridPosition.x == maxX
                                || c.GridPosition.y == minY
                                || c.GridPosition.y == maxY);
        }
    }
}
