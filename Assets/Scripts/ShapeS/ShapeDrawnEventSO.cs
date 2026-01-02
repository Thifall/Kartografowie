using Kartografowie.Shapes;
using System;
using UnityEngine;

namespace Kartografowie
{
    [CreateAssetMenu(fileName = "ShapeDrawnEventSO", menuName = "Events/Shape Drawn Event")]
    public class ShapeDrawnEventSO : ScriptableObject
    {
        public Action<Shape> OnShapeDrawn;

        public void RaiseOnShapeDrawnEvent(Shape shape)
        {
            OnShapeDrawn?.Invoke(shape);
        }
    }
}
