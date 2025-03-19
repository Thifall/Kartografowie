using System;
using UnityEngine;

namespace Kartografowie.Cards
{

    [CreateAssetMenu(menuName = "Events/Card Draw Event")]
    public class CardDrawEventSO : ScriptableObject
    {
        public event Action<DiscoveryCard> OnCardDrawn;
        public void RaiseEvent(DiscoveryCard card)
        {
            if (OnCardDrawn != null)
            {
                OnCardDrawn?.Invoke(card);
            }
        }
    }
}
