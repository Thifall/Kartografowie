using System.Collections;
using Kartografowie.Grid;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Kartografowie.TestsPlayMode
{
    public class GridManagerTests
    {
        private readonly Vector3 POSITION_TRAVERSE = new(1, -5, 0);

        [UnityTest]
        public IEnumerator GridManager_Should_Initialize_Cells_On_Start()
        {
            GameObject gridManagerObject = new("GridManager");
            GridManager gridManager = gridManagerObject.AddComponent<GridManager>();

            GameObject cellObject = new("A1");
            cellObject.transform.position = new Vector3(0, 0, 0) + POSITION_TRAVERSE;
            GridCell cell = cellObject.AddComponent<GridCell>();

            cellObject.transform.SetParent(gridManager.transform);

            // Poczekaj 1 frame, aby Start() siê wykona³o
            yield return null;

            // SprawdŸ, czy siatka siê zainicjalizowa³a
            Assert.IsNotEmpty(gridManager.GetAvailableEmptySquares(), "Grid cells should be initialized in Start().");
            Assert.AreEqual("A1", gridManager.GetCellNameAtPosition(new Vector3(0, 0, 0)));
        }
    }
}
