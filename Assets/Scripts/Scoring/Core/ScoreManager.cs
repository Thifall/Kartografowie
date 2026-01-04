using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.Assets.Scripts.Scoring.Events;
using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.Assets.Scripts.Scoring.Rules.Forests;
using Kartografowie.Assets.Scripts.Scoring.Rules.Misc;
using Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters;
using Kartografowie.Assets.Scripts.Scoring.Rules.Vilages;
using Kartografowie.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Kartografowie.Assets.Scripts.Scoring.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public SeasonEndEventSO SeasonEndEvent;
        public OnScoringRuleAddedEventSO OnScoringRuleAdded;
        public OnScoreUpdatedEventSO OnScoreUpdated;
        public ShapeDrawnEventSO ShapeDrawnEvent;
        public OnCoinAddedEventSO OnCoinAddedEvent;
        public TextMeshProUGUI TotalScore;

        private ScoringContext _scoringContext;
        private GridManager _gridManager;
        private CoinTracker _coinTracker;

        private IEnumerator Start()
        {
            while (_gridManager == null)
            {
                _gridManager = FindFirstObjectByType<GridManager>();
                yield return null;
            }
            _scoringContext = new ScoringContext();
            SeasonEndEvent.OnSeasonEnd += OnSeasonEnd;
            InitializeScoringRules();
            _coinTracker = new CoinTracker(ShapeDrawnEvent, OnCoinAddedEvent, _gridManager);
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
            _scoringContext.RegisterSeasonRules(Seasons.Wiosna, new List<ScoringRule> { edictARule, edictBRule });
            _scoringContext.SetScore(Seasons.Wiosna, edictARule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Wiosna, edictBRule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Wiosna, Edicts.Monesters, 0);
            _scoringContext.SetScore(Seasons.Wiosna, Edicts.Coins, 0);
            _scoringContext.RegisterSeasonRules(Seasons.Lato, new List<ScoringRule> { edictBRule, edictCRule });
            _scoringContext.SetScore(Seasons.Lato, edictBRule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Lato, edictCRule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Lato, Edicts.Monesters, 0);
            _scoringContext.SetScore(Seasons.Lato, Edicts.Coins, 0);
            _scoringContext.RegisterSeasonRules(Seasons.Jesien, new List<ScoringRule> { edictCRule, edictDRule });
            _scoringContext.SetScore(Seasons.Jesien, edictCRule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Jesien, edictDRule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Jesien, Edicts.Monesters, 0);
            _scoringContext.SetScore(Seasons.Jesien, Edicts.Coins, 0);
            _scoringContext.RegisterSeasonRules(Seasons.Zima, new List<ScoringRule> { edictDRule, edictARule });
            _scoringContext.SetScore(Seasons.Zima, edictDRule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Zima, edictARule.EdictType, 0);
            _scoringContext.SetScore(Seasons.Zima, Edicts.Monesters, 0);
            _scoringContext.SetScore(Seasons.Zima, Edicts.Coins, 0);

            //raise events for UI to update
            OnScoringRuleAdded.RaiseEvent(edictARule, Edicts.Edict_A, this);
            OnScoringRuleAdded.RaiseEvent(edictBRule, Edicts.Edict_B, this);
            OnScoringRuleAdded.RaiseEvent(edictCRule, Edicts.Edict_C, this);
            OnScoringRuleAdded.RaiseEvent(edictDRule, Edicts.Edict_D, this);
        }

        private void OnSeasonEnd(Seasons endingSeason, bool isGameOver)
        {
            //EnsureGridManagerInstance();
            //calculate scores for the ending season
            foreach (var rule in _scoringContext.ActiveRules[endingSeason])
            {
                var points = rule.CalculateScore(_gridManager);
                _scoringContext.SetScore(endingSeason, rule.EdictType, points);
                OnScoreUpdated.RaiseEvent(endingSeason, rule.EdictType, points);
            }
            //deduct points from monster tiles
            var monsterSquares = _gridManager.GetSquares(c => c.CurrentCellType == CellType.Monster);
            var visited = new HashSet<Vector2Int>();
            foreach (var square in monsterSquares)
            {
                var cell = square.Value;
                foreach (var neighbor in Generals.Directions.Select(d => cell.GridPosition + d))
                {
                    var neighborCell = _gridManager.GetSquareByPosition(neighbor);
                    if (neighborCell != null && neighborCell.CurrentCellType == CellType.Default)
                    {
                        visited.Add(neighbor);
                    }
                }
            }
            var deductedPoints = visited.Count * -1;
            _scoringContext.SetScore(endingSeason, Edicts.Monesters, deductedPoints);
            OnScoreUpdated.RaiseEvent(endingSeason, Edicts.Monesters, deductedPoints);

            //add bonus points for shapes, sorrounded mountains etc;
            var coinsPoints = _coinTracker.CoinsCount;
            _scoringContext.SetScore(endingSeason, Edicts.Coins, coinsPoints);
            OnScoreUpdated.RaiseEvent(endingSeason, Edicts.Coins, coinsPoints);

            TotalScore.text = $"Total: {_scoringContext.TotalScore}";
            Debug.Log($"Season {endingSeason} ended. Game Over: {isGameOver}");
        }
    }
}
