using Kartografowie.General;
using System;

namespace Kartografowie.Assets.Scripts.Grid.Core
{
    [Serializable]
    public class CellConfig
    {
        public bool HasRuins;
        public CellType InitialCellType = CellType.Default;
    }
}
