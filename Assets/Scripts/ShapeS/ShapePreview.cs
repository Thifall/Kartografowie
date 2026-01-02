using Kartografowie.Assets.Scripts.Grid.Placement;
using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.Cards;
using Kartografowie.General;
using Kartografowie.Shapes;
using Kartografowie.Terrains;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Shapes
{
    public class ShapePreview : MonoBehaviour
    {
        public TerrainSelectedEventSO TerrainSelectedEvent;
        public ShapeSelectedEventSO ShapeSelectedEvent;
        public ForceSingleSquareEventSO ForceSingleSquareEvent;
        public ShapeDrawnEventSO ShapeDrawnEvent;
        public CardDrawEventSO CardDrawEvent;
        private GameObject _currentGhostShape;
        private ShapeSelector _shapeSelector;
        private CellType _currentCellType;
        private GridManager _gridManager;
        private ShapeValidator _shapeValidator;
        private ShapePlacementRules _placementRules;
        private bool _shapeUsed = true;
        private bool _requiresRuins = false;
        private bool _altPressed = false;

        private IEnumerator Start()
        {
            _shapeSelector = FindFirstObjectByType<ShapeSelector>();

            while (_gridManager == null)
            {
                _gridManager = FindFirstObjectByType<GridManager>();
                yield return null;
            }

            _shapeValidator = new ShapeValidator(_gridManager, ShapeDrawnEvent);
            _placementRules = new ShapePlacementRules(_gridManager);
        }

        private void Update()
        {
            if (_shapeUsed)
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
                _altPressed = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                _altPressed = false;
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
            _currentCellType = terrainSelected;
        }

        private void UpdateShapeSelected(Sprite obj)
        {
            if (_currentGhostShape != null)
                Destroy(_currentGhostShape);
        }

        private void OnCardDrawn(DiscoveryCard newCard)
        {
            if (newCard.IsRuins)
            {
                _requiresRuins = true;
                return;
            }


            if (newCard is AmbushCard)
            {
                Debug.Log("Ambush!");
                _shapeValidator.HandleAmbushShape(newCard as AmbushCard);
                _shapeUsed = true;
                return;
            }

            if (!_shapeValidator.CanFitShape(newCard.availableShapes, _requiresRuins))
            {
                ForceSingleSquareEvent.RaiseEvent();
                _requiresRuins = false;
            }

            _shapeUsed = false;
        }

        private void RotateShape()
        {
            if (_currentGhostShape == null)
            {
                return;
            }
            var transforms = _currentGhostShape.GetComponentsInChildren<Transform>()
                .Where(t => t != _currentGhostShape.transform).ToArray();
            foreach (Transform t in transforms)
            {
                t.localPosition = _altPressed ?
                    new Vector3(t.localPosition.y, -t.localPosition.x) :
                    new Vector3(-t.localPosition.y, t.localPosition.x);
            }
        }

        private void FlipShape()
        {
            if (_currentGhostShape == null)
            {
                return;
            }
            var transforms = _currentGhostShape.GetComponentsInChildren<Transform>()
                .Where(t => t != _currentGhostShape.transform).ToArray();
            foreach (Transform t in transforms)
            {
                t.localPosition = new Vector3(-t.localPosition.x, t.localPosition.y);
            }
        }

        private void UpdateGhostShape()
        {
            if (_currentGhostShape == null)
            {

                if (_shapeSelector == null) return;

                GameObject selectedShape = _shapeSelector.GetSelectedShape();
                if (selectedShape == null) return;

                _currentGhostShape = Instantiate(selectedShape);
            }

            _currentGhostShape.transform.position = GetSnappedPosition();
            if (!CanPlaceShape())
            {
                SetGhostColor(Color.red);
            }
            else
            {
                SetGhostColor(Color.white);
                SetGhostTransparency(0.5f);
            }
        }

        private bool CanPlaceShape()
        {
            var positions = _currentGhostShape
               .GetComponentsInChildren<Transform>()
               .Where(t => t != _currentGhostShape.transform)
               .Select(t => t.position);
            return _placementRules.AllowsPlacementAtPositions(positions, _requiresRuins);
        }

        private IEnumerator ConfirmDrawing()
        {
            if (_currentGhostShape == null) yield break;
            if (_shapeUsed) yield break;

            if (!CanPlaceShape())
            {
                Debug.Log("Cannot draw shape here");
                yield break;
            }
            var positions = _currentGhostShape.GetComponentsInChildren<Transform>()
                .Where(t => t != _currentGhostShape.transform)
                .Select(t => t.position)
                .ToArray();
            _currentGhostShape.SetActive(false);
            _shapeUsed = true;
            _requiresRuins = false;
            yield return _gridManager.StartCoroutine(_gridManager.PaintShapeAtWorldPos(positions, _currentCellType));
            //invoke on shape placed event if needed
            ShapeDrawnEvent.RaiseOnShapeDrawnEvent(_currentGhostShape.GetComponent<Shape>());
            _shapeSelector.ResetShape();
            Destroy(_currentGhostShape);
        }

        private Vector3 GetSnappedPosition()
        {
            return _gridManager.GetSquarePositionFromCursorPosition();
        }

        private void SetGhostTransparency(float alpha)
        {
            foreach (SpriteRenderer sr in _currentGhostShape.GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }

        private void SetGhostColor(Color color)
        {
            foreach (SpriteRenderer sr in _currentGhostShape.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = color;
            }
        }

        public bool WasShapePlaced()
        {
            return _shapeUsed;
        }
    }
}
