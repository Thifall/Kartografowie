namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public abstract class VilageScoringRule : ScoringRule
    {
        protected VilageScoringRule(string ruleName, string ruleDescription) : base(ruleName, ruleDescription)
        {
        }
        public override RuleType RuleType => RuleType.Vilages;
    }
}
