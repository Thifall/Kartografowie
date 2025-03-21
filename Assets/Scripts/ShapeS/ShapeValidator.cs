using Kartografowie.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Shapes
{
    public class ShapeValidator
    {
        private readonly GridManager gridManager;

        public ShapeValidator(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        public bool IsOverInvalidCells(GameObject shapePreview, bool requiresRuins)
        {
            Transform[] ghostCells = shapePreview.GetComponentsInChildren<Transform>();
            foreach (Transform cell in ghostCells)
            {
                if (!gridManager.IsWithinGridBounds(cell.position) || gridManager.IsCellRestricted(cell.position))
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
                    if (gridManager.HasRuinsAtPosition(cell.position))
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
            var emptySquares = gridManager.GetAvailableEmptySquares(requiresRuins: requiresRuinsSquare);
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
                        if (CanFitShapeOnSquare(shape, square))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CanFitShapeOnSquare(GameObject shape, GridCell checkedSquare)
        {
            Debug.Log($"Checking square {checkedSquare.transform.localPosition}");

            //Getting shape square positions
            var shapeSquaresBasePositions = GetShapeSquaresBasePositions(shape);
            //starting with no rotation
            var rotation = 0;
            while (rotation < 360)
            {
                Debug.Log($"Shape: {shape.name}, rotation: {rotation},squares base positions:");

                //Offsetting squares by position of our square, to match it on grid (based from [0,0])
                var offsettedPositions = shapeSquaresBasePositions.Select(x => x + checkedSquare.transform.localPosition);

                //our checked square can be any of shapes square, so we have to check for all traverses
                foreach (var traverse in shapeSquaresBasePositions)
                {
                    Debug.Log($"Checking traverse {traverse}");

                    //offset needs to be modified by traverse, to be able to check all combinations of our square being part of shape
                    var traversedAndOffsettedPositions = offsettedPositions.Select(x => x - traverse);

                    //now with such prepared positions, we ask grid manager, if we can draw on those squares
                    if (gridManager.CanDrawOnSquares(traversedAndOffsettedPositions))
                    {
                        Debug.Log("Can draw shape. Match found on positions:");
                        foreach (var squarePosition in traversedAndOffsettedPositions)
                        {
                            Debug.Log($"{gridManager.GetCellNameAtPosition(squarePosition)}");
                        }
                        return true;
                    }
                }
                //rotating base shape
                shapeSquaresBasePositions = RotateShapeSquares(shapeSquaresBasePositions).ToList();
                rotation += 90;
            }
            return false;

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
