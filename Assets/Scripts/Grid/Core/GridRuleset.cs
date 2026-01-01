namespace Kartografowie.Assets.Scripts.Grid.Core
{
    public static class GridRuleset
    {
        public static bool IsCellRestricted(CellState cellState)
        {
            return cellState.IsFilled;
        }   

        public static bool CanDrawOnCell(CellState cellState)
        {
            return !cellState.IsFilled;
        }
    }
}
