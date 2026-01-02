using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.General;
using System.Collections.Generic;
using System.Linq;

namespace Kartografowie.Assets.Scripts.Scoring.Core
{
    public class ScoringContext
    {
        private readonly Dictionary<(Seasons, Edicts), int> _scores = new();
        private readonly Dictionary<Seasons, List<ScoringRule>> _activeRules = new();

        public IReadOnlyDictionary<(Seasons, Edicts), int> Scores => _scores;
        public IReadOnlyDictionary<Seasons, List<ScoringRule>> ActiveRules => _activeRules;

        public int TotalScore => _scores.Values.Sum();

        public void RegisterSeasonRules(Seasons season, List<ScoringRule> rules)
        {
            _activeRules[season] = rules;
        }

        public void SetScore(Seasons season, Edicts edict, int points)
        {
            _scores[(season, edict)] = points;
        }

        public int GetScore(Seasons season, Edicts edict)
        {
            return _scores.TryGetValue((season, edict), out var value) ? value : 0;
        }
    }
}
