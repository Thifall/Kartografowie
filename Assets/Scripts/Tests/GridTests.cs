using System.Collections;
using Kartografowie.General;
using Kartografowie.Grid;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Kartografowie
{
    public class GridTests
    {
        
        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Vilage)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Mountain)]
        public void GridTestsSimplePasses(CellType targetCellType)
        {
            GridCell cell = new();
            Assert.IsTrue(cell.cellType == CellType.Default);

            cell.SetCellType(targetCellType);
            Assert.That(() => cell.cellType == targetCellType);
        }
    }
}
