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
        public void IsEmpty_WhenCellTypeIsDefault_ShouldBeTrue()
        {
            CellState state = new(new CellConfig());
            Assert.IsTrue(state.IsEmpty);
        }

        [Test]
        public void IsFilled_WhenCellTypeIsDefault_ShouldBeFalse()
        {
            CellState state = new(new CellConfig());
            Assert.IsFalse(state.IsFilled);
        }

        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Village)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Mountain)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Monster)]
        public void IsEmpty_WhenCellTypeIsNotDefault_ShouldReturnFalse(CellType cellType)
        {
            CellState state = new(new CellConfig() { InitialCellType = cellType});
            state.SetCellType(cellType);
            Assert.IsFalse(state.IsEmpty);
        }

        [TestCase(CellType.Forest)]
        [TestCase(CellType.Field)]
        [TestCase(CellType.Village)]
        [TestCase(CellType.Water)]
        [TestCase(CellType.Mountain)]
        [TestCase(CellType.Chasm)]
        [TestCase(CellType.Monster)]
        public void IsFilled_WhenCellTypeIsNotDefault_ShouldReturnTrue(CellType cellType)
        {
            CellState state = new(new CellConfig() { InitialCellType = cellType});
            state.SetCellType(cellType);
            Assert.IsTrue(state.IsFilled);
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
