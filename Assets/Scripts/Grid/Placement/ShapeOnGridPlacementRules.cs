using Kartografowie.Assets.Scripts.Grid.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Grid.Placement
{
    public class ShapePlacementRules
    {
        private readonly GridManager grid;

        public ShapePlacementRules(GridManager grid)
        {
            this.grid = grid;
        }

        public bool AllowsPlacementAtPositions(
            IEnumerable<Vector3> worldPositions,
            bool requiresRuins)
        {
            foreach (var pos in worldPositions)
            {
                if (!grid.IsWithinGridBounds(pos))
                    return false;

                if (grid.IsSquareRestricted(pos))
                    return false;
            }

            if (requiresRuins)
            {
                return worldPositions.Any(p => grid.HasRuinsAtPosition(p));
            }

            return true;
        }
    }
}
