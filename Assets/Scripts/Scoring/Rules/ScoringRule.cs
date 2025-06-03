using Kartografowie.Grid;


namespace Kartografowie.Assets.Scripts.Scoring.Rules
{
    public abstract class ScoringRule
    {
        private readonly string RuleName;
        private readonly string RuleDescription;
        public ScoringRule(string ruleName, string ruleDescription)
        {
            RuleName = ruleName;
            RuleDescription = ruleDescription;
        }

        public abstract RuleType RuleType { get; }

        public abstract int CalculateScore(GridManager gridManager);

        public string GetDescription()
        {
            return RuleDescription;
        }
        public string GetName()
        {
            return RuleName;
        }
    }

    public enum RuleType
    {
        Misc,
        Forests,
        Vilages,
        PlainsAndWaters
    }
}
