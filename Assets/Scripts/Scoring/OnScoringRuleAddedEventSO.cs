using Kartografowie.Assets.Scripts.Scoring.Rules;
using Kartografowie.General;
using System;
using System.Collections;
using UnityEngine;

namespace Kartografowie
{
    [CreateAssetMenu(fileName = "OnScoringRuleAddedEvent", menuName = "Events/On Scoring Rule Added")]
    public class OnScoringRuleAddedEventSO : ScriptableObject
    {
        public Action<ScoringRule, Edicts> OnScoringRuleAdded;

        public void RaiseEvent(ScoringRule rule, Edicts edict, MonoBehaviour gameobject)
        {
            gameobject.StartCoroutine(DelayedRaise(rule, edict));
        }

        private IEnumerator DelayedRaise(ScoringRule rule, Edicts edict)
        {
            yield return new WaitUntil(() => OnScoringRuleAdded != null);
            OnScoringRuleAdded?.Invoke(rule, edict);
        }
    }
}
