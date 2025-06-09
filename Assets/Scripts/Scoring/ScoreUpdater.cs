using Kartografowie.General;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kartografowie
{
    public class ScoreUpdater : MonoBehaviour
    {
        public OnScoreUpdatedEventSO onScoreUpdatedEvent;
        public Seasons season;
        public List<Edicts> edictsHandled;
        public List<TextMeshProUGUI> edictPointsTexts;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            onScoreUpdatedEvent.OnScoreUpdated += HandleScoreUpdatedEvent;
        }

        private void HandleScoreUpdatedEvent(Seasons seasons, Edicts edicts, int points)
        {
            if (seasons == season && edictsHandled.Contains(edicts))
            {
                int edictIndex = edictsHandled.IndexOf(edicts);
                if (edictIndex >= 0 && edictIndex < edictPointsTexts.Count)
                {
                    edictPointsTexts[edictIndex].text = points.ToString();
                }
            }
        }
    }
}
