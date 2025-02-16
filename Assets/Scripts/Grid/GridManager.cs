using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 11, gridHeight = 11;
    private Dictionary<Vector2, GridCell> gridCells = new Dictionary<Vector2, GridCell>();
    private float cellSize = 1.05f;
    private Vector3 gridOrigin = new Vector3(1, -5, 0);

    void Start()
    {
        GridCell[] allCells = FindObjectsByType<GridCell>(FindObjectsSortMode.None);
        foreach (GridCell cell in allCells)
        {
            Vector2 pos = WorldToGrid(cell.transform.position);
            gridCells[pos] = cell;
        }
    }

    public bool IsCellRestricted(Vector3 worldPos)
    {
        Vector2 gridPos = WorldToGrid(worldPos);
        return gridCells.ContainsKey(gridPos) && gridCells[gridPos].IsRestricted();
    }

    private Vector2 WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / cellSize);
        return new Vector2(x, y);
    }
    private bool IsWithinGridBounds(Vector2 coords)
    {
        return coords.x >= 0 && coords.x < (gridWidth * cellSize) && coords.y >= 0 && coords.y < (gridHeight * cellSize);
    }

    private GridCell GetCellAt(Vector2 coords)
    {
        return gridCells[coords];
    }

    internal GridCell GetCellAt(int x, int y)
    {
        Vector2 cellCords = new Vector2(x, y);
        return gridCells[cellCords];
    }
}
