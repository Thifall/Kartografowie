using Kartografowie.General;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Vilages
{
    public abstract class VilageScoringRule : ScoringRule
    {
        protected VilageScoringRule(string ruleName, string ruleDescription) : base(ruleName, ruleDescription)
        {
        }
        public override RuleType RuleType => RuleType.Vilages;
        public override Edicts EdictType => Edicts.Edict_C;
    }
}
