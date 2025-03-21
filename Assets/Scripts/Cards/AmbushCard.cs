using System.ComponentModel;

namespace Kartografowie.Cards
{
    public class AmbushCard : DiscoveryCard
    {
        private readonly AmbushStartingCorner startingCorner = AmbushStartingCorner.BOTTOM_LEFT;
    }

    public enum AmbushCheckingDirection
    {
        CLOCKWISE,
        COUNTERCLOCKWISE
    }

    /// <summary>
    /// Intends to select starting corner for shape fit check for ambush cards.
    /// Assuming regular coordinate system, where x rises horizontaly from left to right,
    /// and y rises vertically from bottom to top
    /// </summary>
    public enum AmbushStartingCorner
    {
        /// <summary>
        /// (0, 0) or (xMin, yMin) if 0 is not minimum coordinate value for x and/or y
        /// </summary>
        BOTTOM_LEFT,
        /// <summary>
        /// (xMax, 0) or (xMax, yMin) if 0 is not minimum coordinate value for y
        /// </summary>
        BOTTOM_RIGHT,
        /// <summary>
        /// (0, yMax) or (xMin, yMax) if 0 is not minimum coordinate value for x
        /// </summary>
        TOP_LEFT,
        /// <summary>
        /// (xMax, yMax)
        /// </summary>
        TOP_RIGHT    //(max,max)
    }
}
