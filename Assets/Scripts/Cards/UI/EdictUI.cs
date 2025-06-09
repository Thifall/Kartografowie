using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.General;
using System;
using TMPro;
using UnityEngine;

namespace Kartografowie
{
    public class EdictUI : MonoBehaviour
    {
        public OnScoringRuleAddedEventSO onScoringRuleAdded;
        public TextMeshProUGUI RuleName;
        public TextMeshProUGUI RuleDescription;
        public Edicts EdictType;

        void Start()
        {
            onScoringRuleAdded.OnScoringRuleAdded += HandleScoringRuleAdded;
        }

        private void HandleScoringRuleAdded(ScoringRule rule, Edicts edicts)
        {
           if (edicts == EdictType)
            {
                RuleName.text = rule.GetName();
                RuleDescription.text = rule.GetDescription();
            }
        }
    }
}
