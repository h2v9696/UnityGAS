using System;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects
{
    [Serializable]
    public class GrantTagOnApplying : IAdditionApplyEffect
    {
        /// <summary>
        /// Grant these tag when this GameplayEffect is successfully applied.
        /// will be removed from the Target when effect is removed.
        /// </summary>
        [field: SerializeField]
        public TagSO[] Tags { get; private set; } = Array.Empty<TagSO>();

        public void OnEffectSpecApplied(AbilitySystemComponent target)
        {
            target.TagSystem.AddTags(Tags);
        }

        public void OnEffectSpecRemoved(AbilitySystemComponent target)
        {
            target.TagSystem.RemoveTags(Tags);
        }
    }
}