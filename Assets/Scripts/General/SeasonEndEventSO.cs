using Kartografowie.General;
using System;
using UnityEngine;

namespace Kartografowie
{
    [CreateAssetMenu(menuName = "Events/Season End Event")]
    public class SeasonEndEventSO : ScriptableObject
    {
        public Action<Seasons, bool> OnSeasonEnd;
        public void RaiseEvent(Seasons endingSeason, bool isGameOver)
        {
            OnSeasonEnd?.Invoke(endingSeason, isGameOver);
        }
    }
}
