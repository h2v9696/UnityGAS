using System;
using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.Helper;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.EffectConditions
{
    [Serializable]
    public class TagRequirements : IEffectCondition
    {
        /// <summary>
        /// Tags on the Target that determine if a GameplayEffect can be applied to the Target.
        /// If these requirements are not met, the GameplayEffect is not applied.
        /// </summary>
        [SerializeField, Tooltip("These tags must be present or must not for the effect to be applied.")]
        private TagRequireIgnoreDetails _tagRequirements = new();

        public bool IsPass(GameplayEffectSpec effectSpec)
        {
            return effectSpec.Target.AbilitySystem.HasAllTags(_tagRequirements.RequireTags)
                && effectSpec.Target.AbilitySystem.HasNoneTags(_tagRequirements.IgnoreTags);
        }
    }
}