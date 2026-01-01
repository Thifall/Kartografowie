using Kartografowie.Assets.Scripts.Grid.Core;
using Kartografowie.General;
using NUnit.Framework;

namespace Kartografowie.TestsEditorMode
{
    public class GridCellTestsEditorMode
    {
        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Village)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Mountain)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Monster)]
        public void SetCellType_WhenInvoked_ShouldSetCorrectCellType(CellType targetCellType)
        {
            CellState state = new(new CellConfig());
            Assert.IsTrue(state.CellType == CellType.Default);

            state.SetCellType(targetCellType);
            Assert.AreEqual(targetCellType, state.CellType);
        }

        [Test]
        public void IsRestricted_WhenCellTypeIsDefault_ShouldReturnFalse()
        {
            CellState state = new(new CellConfig());
            Assert.IsFalse(state.IsRestricted());
        }

        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Village)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Mountain)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Monster)]
        public void IsRestricted_WhenCellTypeIsNotDefault_ShouldReturnTrue(CellType cellType)
        {
            CellState state = new(new CellConfig() { InitialCellType = cellType});
            state.SetCellType(cellType);
            Assert.IsTrue(state.IsRestricted());
        }

        [TestCase]
        public void HasRuins_WhenPassedCellConfigWithRuins_ShouldReturnTrue()
        {
            CellState state = new(new CellConfig() { HasRuins = true });
            Assert.IsTrue(state.HasRuins);
        }

        [TestCase]
        public void HasRuins_WhenPassedCellConfigWithoutRuins_ShouldReturnFalse()
        {
            CellState state = new(new CellConfig() { HasRuins = false });
            Assert.IsFalse(state.HasRuins);
        }

    }
}
