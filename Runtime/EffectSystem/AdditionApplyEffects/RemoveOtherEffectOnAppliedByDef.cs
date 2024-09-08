using System;
using H2V.GameplayAbilitySystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects
{
    [Serializable]
    public class RemoveOtherEffectOnAppliedByDef : IAdditionApplyEffect
    {
        /// <summary>
        /// GameplayEffects on the Target that have any of these Defs
        /// will be removed from the Target when this GameplayEffect is successfully applied.
        /// </summary>
        [field: SerializeReference]
        public IGameplayEffectDef[] EffectDefs { get; private set; } = Array.Empty<IGameplayEffectDef>();

        public void OnEffectSpecApplied(AbilitySystemComponent target)
        {
            var effectSystem = target.GameplayEffectSystem;

            foreach (var effectDef in EffectDefs)
            {
                var activeEffect = effectSystem.FindEffectByDef(effectDef);
                target.GameplayEffectSystem.RemoveEffect(activeEffect.Spec);
            }

        }

        public void OnEffectSpecRemoved(AbilitySystemComponent target) {}
    }
}