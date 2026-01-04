using Kartografowie.Assets.Scripts.Scoring.Events;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.UI
{
    public class CoinTrackerUI : MonoBehaviour
    {
        public int _maxCoins = 14;
        private int _coinsFilled = 0;
        public OnCoinAddedEventSO onCoinAddedEvent;
        public GameObject InactiveCoinPrefab;
        public GameObject ActiveCoinPrefab;

        void Start()
        {
            InitializeCoins();
        }

        private void OnEnable()
        {
            onCoinAddedEvent.OnCoinAdded += OnCoinAdded;
        }

        private void OnDisable()
        {
            onCoinAddedEvent.OnCoinAdded -= OnCoinAdded;
        }

        private void OnCoinAdded()
        {
            if (_coinsFilled >= _maxCoins)
            {
                return;
            }
            var toRemove = gameObject.transform.GetChild(_maxCoins - 1);
            if (toRemove != null)
            {
                Destroy(toRemove.gameObject);
            }
            var newCoin = Instantiate(ActiveCoinPrefab, gameObject.transform);
            newCoin.transform.SetSiblingIndex(_coinsFilled);
            _coinsFilled++;
        }

        private void InitializeCoins()
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < _maxCoins; i++)
            {
                var coin = Instantiate(InactiveCoinPrefab, gameObject.transform);
                coin.name = $"Coin {i + 1}";
            }
        }
    }
}
