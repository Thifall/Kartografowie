using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Terrain Selected Event")]
public class TerrainSelectedEventSO : ScriptableObject
{
    public event Action<CellType> OnTerrainSelected;

    public void RaiseEvent(CellType terrainSelected)
    {
        OnTerrainSelected?.Invoke(terrainSelected);
    }
}
