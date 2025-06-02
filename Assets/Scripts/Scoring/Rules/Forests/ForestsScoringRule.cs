namespace Kartografowie.Assets.Scripts.Scoring.Rules.Forests
{
    public abstract class ForestsScoringRule : ScoringRule
    {
        protected ForestsScoringRule(string ruleName, string ruleDescription) : base(ruleName, ruleDescription)
        {
        }

        public override RuleType RuleType => RuleType.Forests;
    }
}
