using Kartografowie.Cards;
using Kartografowie.General;
using Kartografowie.Grid;
using Kartografowie.Terrains;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Shapes
{
    public class ShapePreview : MonoBehaviour
    {
        public TerrainSelectedEventSO TerrainSelectedEvent;
        public ShapeSelectedEventSO ShapeSelectedEvent;
        public ForceSingleSquareEventSO ForceSingleSquareEvent;
        public CardDrawEventSO CardDrawEvent;
        private GameObject currentGhostShape;
        private Vector3 gridOrigin;
        private const float GRID_CELL_SIZE = 1.05f;
        private ShapeSelector shapeSelector;
        private CellType currentCellType;
        private GridManager gridManager;
        private ShapeValidator shapeValidator;
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
            if (currentGhostShape != null)
                Destroy(currentGhostShape);
        }

        private void OnCardDrawn(DiscoveryCard newCard)
        {
            if (newCard.IsRuins)
            {
                requiresRuins = true;
                return;
            }

            if (!shapeValidator.CanFitShape(newCard.availableShapes, requiresRuins))
            {
                ForceSingleSquareEvent.RaiseEvent();
                requiresRuins = false;
            }

            shapeUsed = false;
        }

        void Start()
        {
            shapeSelector = FindFirstObjectByType<ShapeSelector>();
            gridOrigin = new Vector3(1, -5, 0);
        }

        void Update()
        {
            if (gridManager == null)
            {
                var gridman = FindFirstObjectByType<GridManager>();
                if (gridman != null)
                {
                    gridManager = gridman;
                    shapeValidator = new ShapeValidator(gridManager);
                }
            }
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
            if (currentGhostShape == null)
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
            if (currentGhostShape == null)
            {

                if (shapeSelector == null) return;

                GameObject selectedShape = shapeSelector.GetSelectedShape();
                if (selectedShape == null) return;

                currentGhostShape = Instantiate(selectedShape);
            }

            currentGhostShape.transform.position = GetSnappedPosition();
            ApplyGhostTransformations();

            // Sprawdzamy, czy kszta³t nachodzi na niedozwolone pola
            if (shapeValidator.IsOverInvalidCells(currentGhostShape, requiresRuins))
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

            if (shapeValidator.IsOverInvalidCells(currentGhostShape, requiresRuins))
            {
                Debug.Log("Cannot draw shape here");
                return;
            }

            foreach (Transform cell in currentGhostShape.GetComponentsInChildren<Transform>())
            {
                DrawTerrainAtPosition(cell);
            }
            shapeUsed = true;
            requiresRuins = false;
            shapeSelector.ResetShape();
            Destroy(currentGhostShape);
        }

        private void DrawTerrainAtPosition(Transform cell)
        {
            Vector3 snappedPos = new(Mathf.Round((cell.position.x - gridOrigin.x) / GRID_CELL_SIZE),
                                                    Mathf.Round((cell.position.y - gridOrigin.y) / GRID_CELL_SIZE),
                                                    0);
            gridManager.PaintCellAt(snappedPos, currentCellType);
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
}
