using Kartografowie.General;

namespace Kartografowie.Assets.Scripts.Grid.Core
{
    public class CellState
    {
        public CellType CellType { get; private set; }
        public bool HasRuins { get; }
        public bool IsEmpty => CellType == CellType.Default;
        public bool IsFilled => CellType != CellType.Default;

        public CellState(CellConfig cellConfig)
        {
            CellType = cellConfig.InitialCellType;
            HasRuins = cellConfig.HasRuins;
        }

        public void SetCellType(CellType type)
        {
            CellType = type;
        }
    }
}
