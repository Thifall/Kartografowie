using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Force Single Square Event")]
public class ForceSingleSquareEventSO : ScriptableObject
{
    public event Action OnForceSingleSquare;

    public void RaiseEvent()
    {
        OnForceSingleSquare?.Invoke();
    }
}
