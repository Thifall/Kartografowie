using Kartografowie.General;
using System;
using UnityEngine;

namespace Kartografowie.Terrains
{
    [CreateAssetMenu(menuName = "Events/Terrain Selected Event")]
    public class TerrainSelectedEventSO : ScriptableObject
    {
        public event Action<CellType> OnTerrainSelected;

        public void RaiseEvent(CellType terrainSelected)
        {
            OnTerrainSelected?.Invoke(terrainSelected);
        }
    } 
}
