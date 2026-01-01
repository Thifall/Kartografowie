using Kartografowie.General;

namespace Kartografowie.Assets.Scripts.Grid.Core
{
    public class CellState
    {
        public CellType CellType { get; private set; } = CellType.Default;
        public bool HasRuins { get; } = false;

        public CellState(CellConfig cellConfig)
        {
            CellType = cellConfig.InitialCellType;
            HasRuins = cellConfig.HasRuins;
        }

        public void SetCellType(CellType type)
        {
            CellType = type;
        }

        public bool IsRestricted()
        {
            return CellType != CellType.Default;
        }
    }
}
