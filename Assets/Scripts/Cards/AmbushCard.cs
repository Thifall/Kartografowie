using System.ComponentModel;
using UnityEngine;

namespace Kartografowie.Cards
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/New Ambush Card")]
    public class AmbushCard : DiscoveryCard
    {
        [Header("Ambush")]
        public AmbushStartingCorner startingCorner = AmbushStartingCorner.BOTTOM_LEFT;
        public bool clockwiseCheck = false;
    }
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
