using UnityEngine;

public class ShapeValidator{
    private const float GRID_CELL_SIZE = 1.05f;

    private GridManager gridManager;
    private Vector3 gridOrigin;

    public ShapeValidator(GridManager gridManager)
    {
        this.gridManager = gridManager;
        gridOrigin = new Vector3(1, -5, 0);
    }

    public bool IsOverInvalidCells(GameObject shapePreview, bool requiresRuins = false)
    {
        Transform[] ghostCells = shapePreview.GetComponentsInChildren<Transform>();
        foreach (Transform cell in ghostCells)
        {
            if (!IsWithinGridBounds(cell.position) || gridManager.IsCellRestricted(cell.position))
            {
                return true;
            }
        }
        //we're in bounds, and not over restricted squares
        //now we check if ruins are required, we check for at elast one cell hovering ruins square
        if (requiresRuins)
        {
            foreach (Transform cell in ghostCells)
            {
                if (gridManager.HasRuins(cell.position))
                {
                    //we found ruins square, so we return false as we are over good squares
                    return false;
                }
            }
            //if none ruins found, we are over invalid squares
            return true;
        }
        //all checks passed, squares are OK
        return false;
    }

    public bool CanFitShape(GameObject[] availableShapes, bool requiresRuins = false)
    {
        //Scenarios to check:
        //1) Ruins card is active, but all ruins fields are already painted -> force single square shape
        //2) ruins card is active, but there is at least one ruin field open -> check if can fit any of available 
        //   shapes anywhere and force single square if not
        //3) ruins card is inactive, can fit shape anywhere with regular drawing ruleset
        var canFitShape = true;
        if (requiresRuins) //if ruins card is active on shapes
        {
            if (!gridManager.HasAvailableRuinsSquare())//if we don't have any free squares with ruins
            {
                canFitShape = false;
                Debug.Log($"No available ruins squares - should force 1x1 square with chosen type");
                //Force single square of any cell type that can be painted by player
            }
            else
            {
                Debug.Log("Have available ruins squares, checking if any of available shapes from drawn card can fit on grid");
            }
        }

        return canFitShape;
    }

    private bool IsWithinGridBounds(Vector3 position)
    {
        int gridX = Mathf.RoundToInt((position.x - gridOrigin.x) / GRID_CELL_SIZE);
        int gridY = Mathf.RoundToInt((position.y - gridOrigin.y) / GRID_CELL_SIZE);

        return gridX >= 0 && gridX < 11 && gridY >= 0 && gridY < 11; // 11x11 to rozmiar siatki
    }
}
