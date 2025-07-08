using Kartografowie.Cards;
using Kartografowie.General;
using Kartografowie.Grid;
using Kartografowie.Terrains;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Shapes
{
    public class ShapePreview : MonoBehaviour
    {
        public TerrainSelectedEventSO TerrainSelectedEvent;
        public ShapeSelectedEventSO ShapeSelectedEvent;
        public ForceSingleSquareEventSO ForceSingleSquareEvent;
        public ShapeDrawnEventSO ShapeDrawnEvent;
        public CardDrawEventSO CardDrawEvent;
        private GameObject currentGhostShape;
        private ShapeSelector shapeSelector;
        private CellType currentCellType;
        private GridManager gridManager;
        private ShapeValidator shapeValidator;
        private bool shapeUsed = true;
        private bool requiresRuins = false;
        private bool altPressed = false;

        private void Start()
        {
            shapeSelector = FindFirstObjectByType<ShapeSelector>();
        }

        private void Update()
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
                StartCoroutine(ConfirmDrawing());
                return;
            }
            UpdateGhostShape();
        }

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


            if (newCard is AmbushCard)
            {
                Debug.Log("Ambush!");
                shapeValidator.HandleAmbushShape(newCard as AmbushCard);
                shapeUsed = true;
                return;
            }

            if (!shapeValidator.CanFitShape(newCard.availableShapes, requiresRuins))
            {
                ForceSingleSquareEvent.RaiseEvent();
                requiresRuins = false;
            }
            
            shapeUsed = false;
        }

        private void RotateShape()
        {
            if (currentGhostShape == null)
            {
                return;
            }
            var transforms = currentGhostShape.GetComponentsInChildren<Transform>()
                .Where(t => t != currentGhostShape.transform).ToArray();
            foreach (Transform t in transforms)
            {
                t.localPosition = altPressed ?
                    new Vector3(t.localPosition.y, -t.localPosition.x) :
                    new Vector3(-t.localPosition.y, t.localPosition.x);
            }
        }

        private void FlipShape()
        {
            if (currentGhostShape == null)
            {
                return;
            }
            var transforms = currentGhostShape.GetComponentsInChildren<Transform>()
                .Where(t => t != currentGhostShape.transform).ToArray();
            foreach (Transform t in transforms)
            {
                t.localPosition = new Vector3(-t.localPosition.x, t.localPosition.y);
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

        private IEnumerator ConfirmDrawing()
        {
            if (currentGhostShape == null) yield break;

            if (shapeValidator.IsOverInvalidCells(currentGhostShape, requiresRuins))
            {
                Debug.Log("Cannot draw shape here");
                yield break;
            }
            var positions = currentGhostShape.GetComponentsInChildren<Transform>().Where(t => t != currentGhostShape.transform).Select(t => t.position).ToList();
            currentGhostShape.SetActive(false);
            yield return gridManager.StartCoroutine(gridManager.PaintShapeAtWorldPos(positions, currentCellType));
            shapeUsed = true;
            requiresRuins = false;
            //invoke on shape placed event if needed
            ShapeDrawnEvent.RaiseEvent(currentGhostShape.GetComponent<Shape>());
            shapeSelector.ResetShape();
            Destroy(currentGhostShape);
        }

        private Vector3 GetSnappedPosition()
        {
            return gridManager.GetSquarePositionFromCursorPosition();
        }

        private void SetGhostTransparency(float alpha)
        {
            foreach (SpriteRenderer sr in currentGhostShape.GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }

        private void SetGhostColor(Color color)
        {
            foreach (SpriteRenderer sr in currentGhostShape.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = color;
            }
        }

        public bool WasShapePlaced()
        {
            return shapeUsed;
        }
    }
}
