using Kartografowie.Assets.Scripts.Grid.Runtime;
using Kartografowie.Assets.Scripts.Scoring.Core;
using Kartografowie.General;
using System;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Coins
{
    public class CoinsScoringRule : ScoringRule
    {
        private const string _ruleName = "Tor Monet";
        private const string _ruleDescription = "Zdobądź 1 punkty za złotą monetę na torze monet";
        private readonly CoinTrackerUI _coinTracker;

        public CoinsScoringRule(CoinTrackerUI coinTracker) : base(_ruleName, _ruleDescription)
        {
            _coinTracker = coinTracker;
        }

        public override Edicts EdictType => Edicts.Coins;

        public override RuleType RuleType => RuleType.Coins;

        public override int CalculateScore(GridManager gridManager)
        {
            throw new NotImplementedException();
        }
    }
}
