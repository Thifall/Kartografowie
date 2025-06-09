using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.Assets.Scripts.Scoring.Rules.Forests;
using Kartografowie.Assets.Scripts.Scoring.Rules.Misc;
using Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters;
using Kartografowie.Assets.Scripts.Scoring.Rules.Vilages;
using Kartografowie.General;
using Kartografowie.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Kartografowie
{
    public class ScoreManager : MonoBehaviour
    {
        public SeasonEndEventSO SeasonEndEvent;
        public OnScoringRuleAddedEventSO OnScoringRuleAdded;
        public OnScoreUpdatedEventSO OnScoreUpdated;

        private GridManager _gridManager;
        private readonly Dictionary<Seasons, List<ScoringRule>> seasonScoringRules = new();
        private readonly Dictionary<(Seasons, Edicts), int> seasonsScores = new ();

        void Start()
        {
            SeasonEndEvent.OnSeasonEnd += OnSeasonEnd;
            InitializeScoringRules();
        }

        private void InitializeScoringRules()
        {
            // Initialize Forest Scoring Rules
            List<ForestsScoringRule> ForestScoringRules = new()
            {
                new GorskieOstepy(),
                new LesnaWarta(),
                new LesnaWieza(),
                new Zagajnik()
            };

            // Initialize Water and Plain Scoring Rules
            List<PlainsAndWatersScoringRule> WaterAndPlainScoreRules = new()
            {
                new DolinaMagow(),
                new PolnyStaw(),
                new RozlegleNabrzeze(),
                new ZlotySpichlerz()
            };

            // Initialize Village Scoring Rules
            List<VilageScoringRule> VilageScoringRules = new()
            {
                new Kolonia(),
                new Umocnienia(),
                new WielkieMiasto(),
                new ZyzneRowniny()
            };

            // Initialize Misc Scoring Rules
            List<MiscScoringRule> MiscScoringRules = new()
            {
                new Kryjowki(),
                new Pogranicze(),
                new TraktHandlowy(),
                new UtraconeWlosci()
            };

            var edictARule = ForestScoringRules[Random.Range(0, ForestScoringRules.Count)];
            var edictBRule = WaterAndPlainScoreRules[Random.Range(0, WaterAndPlainScoreRules.Count)];
            var edictCRule = VilageScoringRules[Random.Range(0, VilageScoringRules.Count)];
            var edictDRule = MiscScoringRules[Random.Range(0, MiscScoringRules.Count)];

            // Assign rules to seasons
            seasonsScores[(Seasons.Wiosna, edictARule.EdictType)] = 0;
            seasonsScores[(Seasons.Wiosna, edictBRule.EdictType)] = 0;
            seasonsScores[(Seasons.Lato, edictBRule.EdictType)] = 0;
            seasonsScores[(Seasons.Lato, edictCRule.EdictType)] = 0;
            seasonsScores[(Seasons.Jesien, edictCRule.EdictType)] = 0;
            seasonsScores[(Seasons.Jesien, edictDRule.EdictType)] = 0;
            seasonsScores[(Seasons.Zima, edictDRule.EdictType)] = 0;
            seasonsScores[(Seasons.Zima, edictARule.EdictType)] = 0;
            seasonScoringRules[Seasons.Wiosna] = new List<ScoringRule> { edictARule, edictBRule };
            seasonScoringRules[Seasons.Lato] = new List<ScoringRule> { edictBRule, edictCRule };
            seasonScoringRules[Seasons.Jesien] = new List<ScoringRule> { edictCRule, edictDRule };
            seasonScoringRules[Seasons.Zima] = new List<ScoringRule> { edictDRule, edictARule };

            //raise events for UI to update
            OnScoringRuleAdded.RaiseEvent(edictARule, Edicts.Edict_A, this);
            OnScoringRuleAdded.RaiseEvent(edictBRule, Edicts.Edict_B, this);
            OnScoringRuleAdded.RaiseEvent(edictCRule, Edicts.Edict_C, this);
            OnScoringRuleAdded.RaiseEvent(edictDRule, Edicts.Edict_D, this);
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
                OnScoreUpdated.RaiseEvent(endingSeason, rule.EdictType, points);
            }

            Debug.Log($"Season {endingSeason} ended. Game Over: {isGameOver}");
        }
    }
}
