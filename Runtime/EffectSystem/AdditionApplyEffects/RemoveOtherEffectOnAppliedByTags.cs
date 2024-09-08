using System;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects
{
    [Serializable]
    public class RemoveOtherEffectOnAppliedByTags : IAdditionApplyEffect
    {
        /// <summary>
        /// GameplayEffects on the Target that have any of these tags in their Asset Tags or Granted Tags
        /// will be removed from the Target when this GameplayEffect is successfully applied.
        /// </summary>
        [field: SerializeField]
        public TagSO[] Tags { get; private set; } = Array.Empty<TagSO>();

        public void OnEffectSpecApplied(AbilitySystemComponent target)
        {
            foreach (var tag in Tags)
            {
                target.GameplayEffectSystem.ExpireEffectWithTagImmediately(tag);
            }
        }

        public void OnEffectSpecRemoved(AbilitySystemComponent target) {}
    }
}