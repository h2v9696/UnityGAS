using System;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.EffectConditions
{
    [Serializable]
    public class ChanceToApply : IEffectCondition
    {
        [SerializeField, Range(0f, 1f),
        Tooltip("There {ChanceToApply}*100% chance that effect will be applied")]
        private float _chance = 1f;
        
        public bool IsPass(GameplayEffectSpec effectSpec)
        {
            var rndValue = UnityEngine.Random.value;
            var effectDef = effectSpec.EffectDef;
            if (rndValue > _chance)
            {
                Debug.Log($"ChanceToApply::IsPass:: {effectDef.Name} failed to apply " 
                    + $"with chance {_chance} and random value {rndValue}");
                return false;
            }

            return true;
        }
    }
}