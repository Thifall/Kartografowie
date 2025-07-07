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
            if(edictPointsTexts.Count != edictsHandled.Count)
            {
                Debug.LogError("The number of edict points texts does not match the number of edicts handled.");
                return;
            }
            if (edictsHandled.Count == 0)
            {
                Debug.LogWarning("No edicts handled, no points to display.");
                return;
            }
            SetupInitialValues();
        }

        private void SetupInitialValues()
        {
            foreach (var edict in edictsHandled)
            {
                int initialPoints = 0; // Assuming initial points are zero, adjust as necessary
                int edictIndex = edictsHandled.IndexOf(edict);
                if (edictIndex >= 0 && edictIndex < edictPointsTexts.Count)
                {
                    edictPointsTexts[edictIndex].text = EdictScorePointText(edict, initialPoints);
                }
            }
        }

        private void HandleScoreUpdatedEvent(Seasons seasons, Edicts edicts, int points)
        {
            if (seasons == season && edictsHandled.Contains(edicts))
            {
                int edictIndex = edictsHandled.IndexOf(edicts);
                if (edictIndex >= 0 && edictIndex < edictPointsTexts.Count)
                {
                    edictPointsTexts[edictIndex].text = EdictScorePointText(edicts, points);
                }
            }
        }

        private string EdictScorePointText(Edicts edict, int points)
        {
            return $"{edict.EdictName()}: {points}";
        }
    }
}
