using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.Cards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Shapes
{
    public class ShapeValidator
    {
        private readonly GridManager _gridManager;
        private readonly ShapeDrawnEventSO _shapeDrawnEvent;

        public ShapeValidator(GridManager gridManager, ShapeDrawnEventSO shapeDrawnEvent)
        {
            _gridManager = gridManager;
            _shapeDrawnEvent = shapeDrawnEvent;
        }

        public bool IsOverInvalidCells(GameObject shapePreview, bool requiresRuins)
        {
            Transform[] ghostCells = shapePreview.GetComponentsInChildren<Transform>().Where(t => t != shapePreview.transform).ToArray();
            foreach (Transform cell in ghostCells)
            {
                if (!_gridManager.IsWithinGridBounds(cell.position) || _gridManager.IsSquareRestricted(cell.position))
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
                    if (_gridManager.HasRuinsAtPosition(cell.position))
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

        public bool CanFitShape(GameObject[] availableShapes, bool requiresRuinsSquare = false)
        {
            //Scenarios to check:
            //1) Ruins card is active, but all ruins fields are already painted -> force single square shape
            //2) ruins card is active, but there is at least one ruin field open -> check if can fit any of available 
            //   shapes anywhere and force single square if not
            //3) ruins card is inactive, can fit shape anywhere with regular drawing ruleset

            //Take all empty squares, if requiresRuinsSquare == true, then we only take empty ruins squares
            var emptySquares = _gridManager.GetAvailableEmptySquares(requiresRuins: requiresRuinsSquare).ToList();
            if (!emptySquares.Any())
            {
                //we don't have any free squares matching criteria, so we cannot fit shape :)
                Debug.Log($"No available squares - should force 1x1 square with chosen type");
                return false;
            }
            else
            {
                //Checking squares one by one
                foreach (var square in emptySquares)
                {
                    Debug.Log($"checked square: {square.name}, pos:{square.transform.position}, local pos: {square.transform.localPosition}");
                    //checking trough all shapes for that square
                    foreach (var shape in availableShapes)
                    {
                        Debug.Log($"Shape: {shape.name}");
                        if (CanFitShapeOnSquare(shape, square).found)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void HandleAmbushShape(AmbushCard ambushCard)
        {
            if(!CanFitShape(ambushCard.availableShapes))
            {
                Debug.Log("Cannot fit ambush shape anywhere on grid.");
                _shapeDrawnEvent.RaiseOnShapeDrawnEvent(null);
                return;
            }

            Dictionary<Vector2Int, GridCell> emptySquares = _gridManager.GetAvailableEmptySquares()
                .ToDictionary(c => _gridManager.WorldToGrid(c.transform.position));

            //setting bounds
            int minX = emptySquares.Keys.Min(v => v.x);
            int maxX = emptySquares.Keys.Max(v => v.x);
            int minY = emptySquares.Keys.Min(v => v.y);
            int maxY = emptySquares.Keys.Max(v => v.y);

            Vector2Int startingCorner = GetStartingCorner(ambushCard.startingCorner, minX, maxX, minY, maxY);
            Vector2Int[] directions = GetDirections(ambushCard.startingCorner, ambushCard.clockwiseCheck);

            var checkedCells = new List<GridCell>();
            var directionIndex = 0;

            var currentCorner = startingCorner;

            while (emptySquares.Any() && minX < maxX && minY < maxY)
            {
                Debug.Log($"current corner: {currentCorner}, direction: {directions[directionIndex % 4]}");
                foreach (var cell in _gridManager.GetSquaresInLine(emptySquares.Values, directions[directionIndex % 4], currentCorner))
                {
                    var (found, matches) = CanFitShapeOnSquare(ambushCard.availableShapes[0], cell, 90);
                    if (found)
                    {
                        Debug.Log($"can paint shape on square {cell.GridPosition}");
                        _gridManager.StartCoroutine(PaintSquares(ambushCard, matches));
                        return;
                    }
                    emptySquares.Remove(cell.GridPosition);
                }

                currentCorner = GetNextCorner(currentCorner, directions[directionIndex % 4], minX, maxX, minY, maxY);

                //tighten bounds if we done full circle

                directionIndex++;
                if (directionIndex > 0 && directionIndex % 4 == 3)
                {
                    minX++;
                    maxX--;
                    minY++;
                    maxY--;
                    currentCorner = GetStartingCorner(ambushCard.startingCorner, minX, maxX, minY, maxY);
                }
            }
        }

        private IEnumerator PaintSquares(AmbushCard ambushCard, List<Vector3> matches)
        {
            foreach (var position in matches)
            {                
                var cellCords = _gridManager.PositionToGrid(position);
                _gridManager.PaintSquareAtGridPos(cellCords, ambushCard.availableTerrains[0]);
                yield return new WaitForSeconds(.5f);
            }
            _shapeDrawnEvent.RaiseOnShapeDrawnEvent(null);
        }

        public Vector2Int GetStartingCorner(AmbushStartingCorner startingCorner, int minX, int maxX, int minY, int maxY)
        {
            return startingCorner switch
            {
                AmbushStartingCorner.BOTTOM_LEFT => new Vector2Int(minX, minY),
                AmbushStartingCorner.BOTTOM_RIGHT => new Vector2Int(maxX, minY),
                AmbushStartingCorner.TOP_LEFT => new Vector2Int(minX, maxY),
                AmbushStartingCorner.TOP_RIGHT => new Vector2Int(maxX, maxY),
                _ => new Vector2Int(0, 0)
            };
        }

        private Vector2Int[] GetDirections(AmbushStartingCorner startingCorner, bool clockwiseCheck)
        {
            return (startingCorner, clockwiseCheck) switch
            {
                //clockwise options
                (AmbushStartingCorner.BOTTOM_LEFT, true) =>
                    new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left },
                (AmbushStartingCorner.BOTTOM_RIGHT, true) =>
                    new Vector2Int[] { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down },
                (AmbushStartingCorner.TOP_LEFT, true) =>
                    new Vector2Int[] { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up },
                (AmbushStartingCorner.TOP_RIGHT, true) =>
                    new Vector2Int[] { Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right },
                //counterclockwise options
                (AmbushStartingCorner.BOTTOM_LEFT, false) =>
                    new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down },
                (AmbushStartingCorner.BOTTOM_RIGHT, false) =>
                    new Vector2Int[] { Vector2Int.up, Vector2Int.left, Vector2Int.down, Vector2Int.right },
                (AmbushStartingCorner.TOP_LEFT, false) =>
                    new Vector2Int[] { Vector2Int.down, Vector2Int.right, Vector2Int.up, Vector2Int.left },
                (AmbushStartingCorner.TOP_RIGHT, false) =>
                    new Vector2Int[] { Vector2Int.left, Vector2Int.down, Vector2Int.right, Vector2Int.up },
                //should not occur, but never know :)
                _ => new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down }
            };
        }

        private Vector2Int GetNextCorner(Vector2Int currentCorner, Vector2Int direction, int minX, int maxX, int minY, int maxY)
        {
            var nextCorner = currentCorner;
            bool IsCorner(Vector2Int pos) =>
                pos.x == minX && pos.y == maxY || //top left
                pos.x == maxX && pos.y == maxY || //top right
                pos.x == minX && pos.y == minY || //bottom left
                pos.x == maxX && pos.y == minY; //bottom right

            do
            {
                nextCorner += direction; //postêp w konkretnym kierunku
            }
            while (!IsCorner(nextCorner) && nextCorner.x <= maxX && nextCorner.x >= minX && nextCorner.y <= maxY && nextCorner.y >= minY);
            return nextCorner;
        }

        private (bool found, List<Vector3> matches) CanFitShapeOnSquare(GameObject shape, GridCell checkedSquare, int maxRotation = 360)
        {
            Debug.Log($"Checking square {checkedSquare.transform.localPosition}");

            //Getting shape square positions
            var shapeSquaresBasePositions = GetShapeSquaresBasePositions(shape).ToList();
            //starting with no rotation
            var rotation = 0;
            while (rotation < maxRotation)
            {
                Debug.Log($"Shape: {shape.name}, rotation: {rotation},squares base positions:");
                foreach (var square in shapeSquaresBasePositions)
                {
                    Debug.Log(square);
                }

                //Offsetting squares by position of our square, to match it on grid (based from [0,0])
                var offsettedPositions = shapeSquaresBasePositions.Select(x => x + checkedSquare.transform.localPosition);

                //our checked square can be any of shapes square, so we have to check for all traverses
                foreach (var traverse in shapeSquaresBasePositions)
                {
                    Debug.Log($"Checking traverse {traverse}");

                    //offset needs to be modified by traverse, to be able to check all combinations of our square being part of shape
                    var traversedAndOffsettedPositions = offsettedPositions.Select(x => x - traverse).ToList();

                    //now with such prepared positions, we ask grid manager, if we can draw on those squares
                    if (_gridManager.CanDrawOnSquares(traversedAndOffsettedPositions))
                    {
                        Debug.Log("Can draw shape. Match found on positions:");
                        foreach (var squarePosition in traversedAndOffsettedPositions)
                        {
                            Debug.Log($"{_gridManager.GetSquareNameAtPosition(squarePosition)}");
                        }
                        return (true, traversedAndOffsettedPositions);
                    }
                }
                //rotating base shape
                shapeSquaresBasePositions = RotateShapeSquares(shapeSquaresBasePositions).ToList();
                rotation += 90;
            }
            return (false, new List<Vector3>());
        }

        private IEnumerable<Vector3> GetShapeSquaresBasePositions(GameObject shape)
        {
            return shape.GetComponentsInChildren<Transform>().Where(t => t != shape.transform).Select(x => x.position).Distinct();
        }

        private IEnumerable<Vector3> RotateShapeSquares(IEnumerable<Vector3> shapeSquares)
        {
            return shapeSquares.Select(v => new Vector3(-v.y, v.x, 0f));
        }
    }
}
