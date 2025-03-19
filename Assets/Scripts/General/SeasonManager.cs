using Kartografowie.Cards;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kartografowie.General
{
    public class SeasonManager : MonoBehaviour
    {
        public TextMeshProUGUI ProgressText;
        public TextMeshProUGUI SeasonText;
        public Image seasonProgressFill;
        public Slider seasonProgressSlider;
        private Dictionary<Seasons, int> seasonLimits;
        private int currentSeason = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            seasonLimits = new Dictionary<Seasons, int> {
            { Seasons.Wiosna, 8 },
            { Seasons.Lato, 8 },
            { Seasons.Jesie�, 7 },
            { Seasons.Zima, 6 },
        };
            seasonProgressSlider.value = 0;
            UpdateSeasonUITexts();
        }

        public void ProgressSeason(int cardProgress)
        {
            seasonProgressSlider.value += cardProgress;
            UpdateSeasonUITexts();
        }

        void UpdateSeasonUITexts()
        {
            ProgressText.text = $"POST�P PORY ROKU: {seasonProgressSlider.value}/{seasonProgressSlider.maxValue}";
            SeasonText.text = $"{Enum.GetName(typeof(Seasons), currentSeason)}";

            // Zmienianie koloru paska post�pu w zale�no�ci od sezonu
            switch ((Seasons)currentSeason)
            {
                case Seasons.Wiosna: seasonProgressFill.color = Color.green; break;
                case Seasons.Lato: seasonProgressFill.color = Color.yellow; break;
                case Seasons.Jesie�: seasonProgressFill.color = new Color(1f, 0.5f, 0f); break; // Pomara�czowy
                case Seasons.Zima: seasonProgressFill.color = Color.cyan; break;
            }
        }

        internal bool IsSeasonOver()
        {
            Seasons season = (Seasons)currentSeason;
            return seasonProgressSlider.value >= seasonLimits[season];
        }

        internal void NextSeason()
        {
            if (!IsSeasonOver()) return;

            Debug.Log("Nowy sezon");
            currentSeason++;
            if (currentSeason >= seasonLimits.Keys.Count)
            {
                Debug.Log("Gra si� sko�czy�a!");
                FindFirstObjectByType<DiscoveryDeckManager>().OnSeasonEnd(true); //TODO: Should not have that reference here
                return; // Mo�esz doda� ekran ko�ca gry
            }

            Seasons newSeason = (Seasons)currentSeason;
            seasonProgressSlider.value = 0;
            seasonProgressSlider.maxValue = seasonLimits[newSeason];
            UpdateSeasonUITexts();

            FindFirstObjectByType<DiscoveryDeckManager>().OnSeasonEnd(false);
        }
    } 
}
