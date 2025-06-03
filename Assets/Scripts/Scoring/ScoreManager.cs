using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.Assets.Scripts.Scoring.Rules.Forests;
using Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters;
using Kartografowie.General;
using Kartografowie.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Kartografowie
{
    public class ScoreManager : MonoBehaviour
    {
        public SeasonEndEventSO SeasonEndEvent;
        public List<ScoringRule> WaterAndPlainScoreRules = new();
        public List<ScoringRule> VilageScoringRules = new();
        public List<ScoringRule> ForestScoringRules = new();
        public List<ScoringRule> MiscScoringRules = new();

        private GridManager _gridManager;
        private readonly Dictionary<Seasons, List<ScoringRule>> seasonScoringRules = new();

        void Start()
        {
            SeasonEndEvent.OnSeasonEnd += OnSeasonEnd;
            InitializeScoringRules();
        }

        private void InitializeScoringRules()
        {
            InitializeForestScoringRules();
            var forestRule = ForestScoringRules[Random.Range(0, ForestScoringRules.Count)];
            seasonScoringRules[Seasons.Wiosna] = new List<ScoringRule> { forestRule };
        }

        private void InitializeForestScoringRules()
        {
            ForestScoringRules.Add(new RozlegleNabrzeze());
        }

        private void OnSeasonEnd(Seasons endingSeason, bool isGameOver)
        {
            if (_gridManager == null)
            {
                _gridManager = FindFirstObjectByType<GridManager>();
            }

            foreach (var rule in seasonScoringRules[endingSeason])
            {
                var points = rule.CalculateScore(_gridManager);
            }

            Debug.Log($"Season {endingSeason} ended. Game Over: {isGameOver}");
        }
    }
}
