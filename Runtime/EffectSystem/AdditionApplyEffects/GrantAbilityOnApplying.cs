using System;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Components;
using UnityEngine;


namespace H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects
{
    [Serializable]
    public class GrantAbilityOnApplying : IAdditionApplyEffect
    {
        /// <summary>
        /// Grant these abilities to the Target when this GameplayEffect is successfully applied.
        /// will be removed from the Target when effect is removed.
        /// </summary>
        [field: SerializeField]
        public AbilitySO[] Abilities { get; private set; } = Array.Empty<AbilitySO>();

        public void OnEffectSpecApplied(AbilitySystemComponent target)
        {
            foreach (var ability in Abilities)
            {
                target.AbilitySystem.GiveAbility(ability);
            }
        }

        public void OnEffectSpecRemoved(AbilitySystemComponent target)
        {
            foreach (var ability in Abilities)
            {
                target.AbilitySystem.RemoveAbility(ability);
            }
        }
    }
}