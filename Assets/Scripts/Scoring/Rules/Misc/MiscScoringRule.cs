using Kartografowie.General;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.Misc
{
    public abstract class MiscScoringRule : ScoringRule
    {
        protected MiscScoringRule(string ruleName, string ruleDescription) : base(ruleName, ruleDescription)
        {
        }
        public override RuleType RuleType => RuleType.Misc;
        public override Edicts EdictType => Edicts.Edict_D;
    }
}
