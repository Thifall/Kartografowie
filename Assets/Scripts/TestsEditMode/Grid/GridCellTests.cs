using Kartografowie.General;
using Kartografowie.Grid;
using NUnit.Framework;

namespace Kartografowie.TestsEditorMode
{
    public class GridCellTests
    {
        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Vilage)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Mountain)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Monster)]
        public void SetCellType_CellType_Should_Match_Target_CellType(CellType targetCellType)
        {
            GridCell cell = new();
            Assert.IsTrue(cell.CellType == CellType.Default);

            cell.SetCellType(targetCellType);
            Assert.That(() => cell.CellType == targetCellType);
        }

        [Test]
        public void IsRestricted_CellType_Default_Should_Be_False()
        {
            GridCell cell = new();
            Assert.IsFalse(cell.IsRestricted());
        }

        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Vilage)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Mountain)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Monster)]
        public void IsRestricted_CellType_NonDefault_Should_Be_True(CellType cellType)
        {
            GridCell cell = new();
            cell.SetCellType(cellType);
            Assert.IsTrue(cell.IsRestricted());
        }

    }
}
