using Kartografowie.General;

namespace Kartografowie.Assets.Scripts.Scoring.Rules.PlainsAndWaters
{
    public abstract class PlainsAndWatersScoringRule : ScoringRule
    {
        public PlainsAndWatersScoringRule(string ruleName, string ruleDescription) : base(ruleName, ruleDescription)
        {
        }

        public override RuleType RuleType => RuleType.PlainsAndWaters;
        public override Edicts EdictType => Edicts.Edict_B;
    }
}
