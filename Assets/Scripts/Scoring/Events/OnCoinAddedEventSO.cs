using System;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Events
{
    [CreateAssetMenu(fileName = "OnCoinAddedEventSO", menuName = "Events/On Coin Added")]
    public class OnCoinAddedEventSO : ScriptableObject
    {
        public Action OnCoinAdded;

        public void RaiseOnCoinAddedEvent()
        {
            OnCoinAdded?.Invoke();
        }
    }
}
