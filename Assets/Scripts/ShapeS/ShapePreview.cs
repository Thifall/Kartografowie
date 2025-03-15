using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapePreview : MonoBehaviour
{
    public TerrainSelectedEventSO TerrainSelectedEvent;
    public ShapeSelectedEventSO ShapeSelectedEvent;
    public CardDrawEventSO CardDrawEvent;
    private GameObject currentGhostShape;
    private Vector3 gridOrigin;
    private const float GRID_CELL_SIZE = 1.05f;
    private ShapeSelector shapeSelector;
    private CellType currentCellType;
    private GridManager gridManager;
    private bool shapeUsed = true;
    private bool requiresRuins = false;
    private int shapeRotation = 0;
    private bool isFlipped = false;
    private bool altPressed = false;

    private void OnEnable()
    {
        TerrainSelectedEvent.OnTerrainSelected += UpdateTerrainSelected;
        ShapeSelectedEvent.OnShapeSelected += UpdateShapeSelected;
        CardDrawEvent.OnCardDrawn += OnCardDrawn;
    }

    private void OnDisable()
    {
        TerrainSelectedEvent.OnTerrainSelected -= UpdateTerrainSelected;
        ShapeSelectedEvent.OnShapeSelected -= UpdateShapeSelected;
        CardDrawEvent.OnCardDrawn -= OnCardDrawn;
    }

    private void UpdateTerrainSelected(CellType terrainSelected)
    {
        currentCellType = terrainSelected;
    }

    private void UpdateShapeSelected(Sprite obj)
    {
        //shapeUsed = false;
    }

    private void OnCardDrawn(DiscoveryCard newCard)
    {
        if (newCard.IsRuins)
        {
            requiresRuins = true;
            return;
        }
        CanFitShape(newCard.availableShapes);
        shapeUsed = false;
    }

    private void CanFitShape(GameObject[] availableShapes)
    {
        //throw new NotImplementedException();
    }

    void Start()
    {
        shapeSelector = FindFirstObjectByType<ShapeSelector>();
        gridManager = FindFirstObjectByType<GridManager>();
        gridOrigin = new Vector3(1, -5, 0);
    }

    void Update()
    {
        if (shapeUsed)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateShape();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FlipShape();
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            altPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            altPressed = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            ConfirmDrawing();
            return;
        }
        UpdateGhostShape();
    }

    private void RotateShape()
    {
        if (currentGhostShape is null)
        {
            return;
        }
        int rotation = 90;
        if (altPressed)
        {
            rotation = -rotation;
        }
        shapeRotation = (shapeRotation + rotation) % 360;
    }

    private void FlipShape()
    {
        if (currentGhostShape != null)
        {
            isFlipped = !isFlipped;
        }
    }

    private void UpdateGhostShape()
    {
        if (currentGhostShape != null)
            Destroy(currentGhostShape);

        GameObject selectedShape = shapeSelector.GetSelectedShape();
        if (selectedShape == null) return;

        // Tworzymy "ducha" kszta³tu
        currentGhostShape = Instantiate(selectedShape);
        currentGhostShape.transform.position = GetSnappedPosition();

        ApplyGhostTransformations();

        // Sprawdzamy, czy kszta³t nachodzi na niedozwolone pola
        if (IsOverInvalidCells(currentGhostShape))
        {
            SetGhostColor(Color.red);
        }
        else
        {
            SetGhostColor(Color.white);
            SetGhostTransparency(0.5f);
        }
    }

    private void ApplyGhostTransformations()
    {
        currentGhostShape.transform.rotation = Quaternion.Euler(0f, 0f, shapeRotation);
        float flipValue = isFlipped ? -1f : 1f;
        currentGhostShape.transform.localScale = new Vector3(flipValue, 1f, 1f);
    }

    void ConfirmDrawing()
    {
        if (currentGhostShape == null) return;

        if (IsOverInvalidCells(currentGhostShape))
        {
            Debug.Log("Cannot draw shape here");
            return;
        }

        foreach (Transform cell in currentGhostShape.GetComponentsInChildren<Transform>())
        {
            GridCell gridCell = GetGridCellAtPosition(cell.position);
            if (gridCell != null)
            {
                gridCell.SetCellType(currentCellType);
            }
        }
        shapeUsed = true;
        requiresRuins = false;
        shapeSelector.ResetShape();
        Destroy(currentGhostShape);
    }

    GridCell GetGridCellAtPosition(Vector3 position)
    {
        Vector3 snappedPos = new Vector3(Mathf.Round((position.x - gridOrigin.x) / GRID_CELL_SIZE),
                                        Mathf.Round((position.y - gridOrigin.y) / GRID_CELL_SIZE),
                                        0);
        return gridManager.GetCellAt((int)snappedPos.x, (int)snappedPos.y);
    }

    Vector3 GetSnappedPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float localX = (mousePos.x - gridOrigin.x) / GRID_CELL_SIZE;
        float localY = (mousePos.y - gridOrigin.y) / GRID_CELL_SIZE;

        int gridX = Mathf.RoundToInt(localX);
        int gridY = Mathf.RoundToInt(localY);

        return new Vector3(gridOrigin.x + gridX * GRID_CELL_SIZE, gridOrigin.y + gridY * GRID_CELL_SIZE, 0);
    }

    bool IsOverInvalidCells(GameObject ghost)
    {
        if (gridManager is null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }
        Transform[] ghostCells = ghost.GetComponentsInChildren<Transform>();
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

    bool IsWithinGridBounds(Vector3 position)
    {
        int gridX = Mathf.RoundToInt((position.x - gridOrigin.x) / GRID_CELL_SIZE);
        int gridY = Mathf.RoundToInt((position.y - gridOrigin.y) / GRID_CELL_SIZE);

        return gridX >= 0 && gridX < 11 && gridY >= 0 && gridY < 11; // 11x11 to rozmiar siatki
    }

    void SetGhostTransparency(float alpha)
    {
        foreach (SpriteRenderer sr in currentGhostShape.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    void SetGhostColor(Color color)
    {
        foreach (SpriteRenderer sr in currentGhostShape.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }

    internal IEnumerable<Vector2> GetCellOffsets()
    {
        var SRs = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        return SRs.Select((r) => new Vector2(r.transform.position.x, r.transform.position.y));
    }

    public bool WasShapePlaced()
    {
        return shapeUsed;
    }
}
