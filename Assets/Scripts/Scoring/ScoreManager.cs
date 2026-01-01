using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.Assets.Scripts.Scoring.Rules.Forests;
using Kartografowie.Assets.Scripts.Scoring.Rules.Misc;
using Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters;
using Kartografowie.Assets.Scripts.Scoring.Rules.Vilages;
using Kartografowie.General;
using Kartografowie.Shapes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Kartografowie
{
    public class ScoreManager : MonoBehaviour
    {
        public SeasonEndEventSO SeasonEndEvent;
        public OnScoringRuleAddedEventSO OnScoringRuleAdded;
        public OnScoreUpdatedEventSO OnScoreUpdated;
        public ShapeDrawnEventSO ShapeDrawnEvent;
        public TextMeshProUGUI TotalScore;
        public GameObject EmptyCoinPrefab;
        public GameObject FilledCoinPrefab;
        public GameObject GoldCoinTrack;

        private const int maxCoins = 14;
        private GridManager _gridManager;
        private readonly Dictionary<Seasons, List<ScoringRule>> seasonScoringRules = new();
        private readonly Dictionary<(Seasons, Edicts), int> seasonsScores = new();
        private readonly Dictionary<Vector2Int, GridCell> visitedMountains = new();
        private int coinsFilled = 0;

        void Start()
        {
            SeasonEndEvent.OnSeasonEnd += OnSeasonEnd;
            ShapeDrawnEvent.OnShapeDrawn += OnShapeDrawn;
            InitializeCoins();
            InitializeScoringRules();
        }

        private void InitializeCoins()
        {
            foreach (Transform child in GoldCoinTrack.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < maxCoins; i++)
            {
                var coin = Instantiate(EmptyCoinPrefab, GoldCoinTrack.transform);
                coin.name = $"Coin {i + 1}";
            }
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
            seasonsScores[(Seasons.Wiosna, Edicts.Monesters)] = 0;
            seasonsScores[(Seasons.Wiosna, Edicts.Coins)] = 0;
            seasonsScores[(Seasons.Lato, edictBRule.EdictType)] = 0;
            seasonsScores[(Seasons.Lato, edictCRule.EdictType)] = 0;
            seasonsScores[(Seasons.Lato, Edicts.Monesters)] = 0;
            seasonsScores[(Seasons.Lato, Edicts.Coins)] = 0;
            seasonsScores[(Seasons.Jesien, edictCRule.EdictType)] = 0;
            seasonsScores[(Seasons.Jesien, edictDRule.EdictType)] = 0;
            seasonsScores[(Seasons.Jesien, Edicts.Monesters)] = 0;
            seasonsScores[(Seasons.Jesien, Edicts.Coins)] = 0;
            seasonsScores[(Seasons.Zima, edictDRule.EdictType)] = 0;
            seasonsScores[(Seasons.Zima, edictARule.EdictType)] = 0;
            seasonsScores[(Seasons.Zima, Edicts.Monesters)] = 0;
            seasonsScores[(Seasons.Zima, Edicts.Coins)] = 0;
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
            EnsureGridManagerInstance();
            //calculate scores for the ending season
            foreach (var rule in seasonScoringRules[endingSeason])
            {
                var points = rule.CalculateScore(_gridManager);
                seasonsScores[(endingSeason, rule.EdictType)] = points;
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
            seasonsScores[(endingSeason, Edicts.Monesters)] = deductedPoints;
            OnScoreUpdated.RaiseEvent(endingSeason, Edicts.Monesters, deductedPoints);

            //add bonus points for shapes, sorrounded mountains etc;
            seasonsScores[(endingSeason, Edicts.Coins)] = coinsFilled;
            OnScoreUpdated.RaiseEvent(endingSeason, Edicts.Coins, coinsFilled);

            TotalScore.text = $"Total: {seasonsScores.Values.Sum()}";
            Debug.Log($"Season {endingSeason} ended. Game Over: {isGameOver}");
        }

        private void EnsureGridManagerInstance()
        {
            if (_gridManager == null)
            {
                _gridManager = FindFirstObjectByType<GridManager>();
            }
        }

        private void OnShapeDrawn(Shape shape)
        {
            if (shape.IsBonusShape)
            {
                AddCoinToTracker();
            }
            Debug.Log($"Shape drawn: {shape.name}. Is bonus shape: {shape.IsBonusShape}");
            CheckForSurroundedMountains();
        }

        private void AddCoinToTracker()
        {
            var toRemove = GoldCoinTrack.transform.GetChild(maxCoins - 1);
            if (toRemove != null)
            {
                Destroy(toRemove.gameObject);
            }
            var newCoin = Instantiate(FilledCoinPrefab, GoldCoinTrack.transform);
            newCoin.transform.SetSiblingIndex(coinsFilled);
            coinsFilled++;
        }

        private void CheckForSurroundedMountains()
        {
            EnsureGridManagerInstance();
            var mountains = _gridManager.GetSquares(c => c.CurrentCellType == CellType.Mountain).ToList();
            foreach (var square in mountains)
            {
                var cell = square.Value;
                bool isSurrounded = true;
                foreach (var neighbor in Generals.Directions.Select(d => cell.GridPosition + d))
                {
                    var neighborCell = _gridManager.GetSquareByPosition(neighbor);
                    if (neighborCell != null && neighborCell.CurrentCellType == CellType.Default)
                    {
                        isSurrounded = false; // Found an empty neighbor, not surrounded
                        break;
                    }
                }
                if (isSurrounded && !visitedMountains.ContainsKey(cell.GridPosition))
                {
                    Debug.Log($"Mountain at {cell.GridPosition} is surrounded by other mountains.");
                    AddCoinToTracker();
                    visitedMountains[cell.GridPosition] = cell; // Mark this mountain as visited
                }
            }
        }
    }
}
