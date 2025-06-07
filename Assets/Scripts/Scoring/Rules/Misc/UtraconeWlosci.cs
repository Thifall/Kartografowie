using Kartografowie.General;
using Kartografowie.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Misc
{
    public class UtraconeWlosci : MiscScoringRule
    {
        private const string RULE_NAME = "Utracone Włości";
        private const string RULE_DESCRIPTION = "Zdobądź 3 punkty za każdy obszar wzdłuż jednej krawędzi największego kwadratu złożonego z wypełnionych obszarów";
        public UtraconeWlosci() : base(RULE_NAME, RULE_DESCRIPTION)
        {
        }

        public override int CalculateScore(GridManager gridManager)
        {
            var points = 0;

            var (minX, maxX, minY, maxY) = gridManager.GetGridBounds();
            List<List<Vector2Int>> filledSquares = new();
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    Vector2Int checkStartPosition = new(x, y); // Start sprawdzania od tej pozycji
                    var hasOnlyFilledCells = true; // Zakładamy, że wszystkie kratki są wypełnione
                    List<Vector2Int> currentSquareElements = new(); // Lista do przechowywania wypełnionych komórek
                    while (hasOnlyFilledCells && checkStartPosition.x <= maxX && checkStartPosition.y <= maxY)
                    {
                        Debug.Log(GetName() + $" Sprawdzanie pozycji: {checkStartPosition}");
                        if (gridManager.GetSquareByPosition(checkStartPosition).CellType == CellType.Default
                            || gridManager.GetSquareByPosition(checkStartPosition).CellType == CellType.Chasm)
                        {
                            hasOnlyFilledCells = false;
                            Debug.Log(GetName() + $" Pozycja {checkStartPosition} nie jest wypełniona.");
                        }
                        else
                        {
                            List<Vector2Int> currentEdgeCheck = new();
                            var checkLeft = checkStartPosition;
                            var checkDown = checkStartPosition;
                            while (checkLeft.x > x)
                            {
                                checkLeft += Vector2Int.left; // Sprawdzamy w lewo
                                checkDown += Vector2Int.down; // Sprawdzamy w dół
                                if (gridManager.GetSquareByPosition(checkLeft).CellType == CellType.Default
                                    || gridManager.GetSquareByPosition(checkLeft).CellType == CellType.Chasm
                                    || gridManager.GetSquareByPosition(checkDown).CellType == CellType.Default
                                    || gridManager.GetSquareByPosition(checkDown).CellType == CellType.Chasm)
                                {
                                    hasOnlyFilledCells = false;
                                }
                                else
                                {
                                    // Dodajemy wypełnione komórki do listy
                                    currentEdgeCheck.Add(checkLeft);
                                    currentEdgeCheck.Add(checkDown);
                                }
                            }
                            if (hasOnlyFilledCells)
                            {
                                currentSquareElements.Add(checkStartPosition);
                                currentSquareElements.AddRange(currentEdgeCheck); // Dodajemy sprawdzone komórki do listy
                            }
                            checkStartPosition += new Vector2Int(1, 1); //Przesuwamy się do następnej pozycji po skosie do góry w prawo - przekątna
                        }
                    }
                    if (currentSquareElements.Count > 0)
                    {
                        filledSquares.Add(currentSquareElements); // Dodajemy listę wypełnionych komórek do głównej listy
                        Debug.Log(GetName() + $" Znaleziono kwadrat wypełnionych komórek o rozmiarze {currentSquareElements.Count}");
                    }
                }
            }

            Debug.Log($"Znaleziono {filledSquares.Count} kwadratów złożonych z wypełnionych obszarów.");
            filledSquares = filledSquares.OrderByDescending(c => c.Count).ToList();
            Debug.Log($"Największy kwadrat ma {filledSquares.First().Count} wypełnionych komórek.");
            float largestSquareCount = filledSquares.First().Count;
            points += Mathf.RoundToInt(Mathf.Sqrt(largestSquareCount)) * 3;

            Debug.Log($"Punkty za regułę \"{RULE_NAME}\": {points}");
            return points;
        }
    }
}
