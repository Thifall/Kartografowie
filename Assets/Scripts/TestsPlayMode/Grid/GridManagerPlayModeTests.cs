using System.Collections;
using Kartografowie.General;
using Kartografowie.Grid;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Kartografowie.TestsPlayMode
{
    public class GridManagerPlayModeTests
    {

        [UnityTest]
        public IEnumerator GridManager_Should_Initialize_Cells_On_Start()
        {
            GameObject gridManagerObject = new("GridManager");
            GridManager gridManager = gridManagerObject.AddComponent<GridManager>();

            GameObject cellObject = new("A1");
            cellObject.transform.position = new Vector3(0, 0, 0);
            cellObject.AddComponent<GridCell>();

            cellObject.transform.SetParent(gridManager.transform);

            yield return null; //delay 1 frame

            Assert.IsNotEmpty(gridManager.GetAvailableEmptySquares(), "Grid cells should be initialized in Start().");
            Assert.AreEqual("A1", gridManager.GetSquareNameAtPosition(new Vector3(0, 0, 0)));
        }

        [UnityTest]
        public IEnumerator GridManager_Should_Paint_Correct_Cell()
        {
            GameObject parentObject = new("parent");
            parentObject.transform.position = new(1, 0, 0);

            GameObject gridManagerObject = new("GridManager");
            GridManager gridManager = gridManagerObject.AddComponent<GridManager>();
            gridManager.transform.SetParent(parentObject.transform);

            GameObject cellObject = new("TestCell");
            cellObject.transform.position = new Vector3(0, 0, 0);
            GridCell gridCell = cellObject.AddComponent<GridCell>();

            cellObject.transform.SetParent(gridManagerObject.transform);

            yield return null; 

            Assert.AreEqual(CellType.Default, gridCell.CellType, "Starting celltype is default");

            gridManager.PaintSquareAtWorldPos(new Vector2(0, 0), CellType.Forest);

            Assert.AreEqual(CellType.Forest, gridCell.CellType, "Celltype should be changed");
        }
    }
}
