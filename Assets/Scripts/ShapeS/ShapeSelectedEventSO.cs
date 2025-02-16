using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/Shape Selected Event")]
public class ShapeSelectedEventSO : ScriptableObject
{
    public event Action<Sprite> OnShapeSelected;

    public void RaiseEvent(Sprite shapeIcon)
    {
        OnShapeSelected?.Invoke(shapeIcon);
    }
}
