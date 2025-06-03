namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public abstract class VillageScoringRule : ScoringRule
    {
        protected VillageScoringRule(string ruleName, string ruleDescription) : base(ruleName, ruleDescription)
        {
        }
        override RuleType RuleType => RuleType.Village;
    }
}
