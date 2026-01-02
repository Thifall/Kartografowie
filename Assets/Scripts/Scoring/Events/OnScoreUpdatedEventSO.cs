using Kartografowie.General;
using System;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Events
{
    [CreateAssetMenu(fileName = "OnScoreUpdatedEventSO", menuName = "Events/On Score Updated")]
    public class OnScoreUpdatedEventSO : ScriptableObject
    {
        public Action<Seasons, Edicts, int> OnScoreUpdated;

        public void RaiseEvent(Seasons season, Edicts edict, int score)
        {
            OnScoreUpdated?.Invoke(season, edict, score);
        }
    }
}
