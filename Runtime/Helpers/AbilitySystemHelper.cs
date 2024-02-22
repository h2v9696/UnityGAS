using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.AbilitySystem.Components;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;

namespace H2V.GameplayAbilitySystem.Helper
{
    public static class AbilitySystemHelper
    {
        /// <summary>
        /// Checks if an Ability System has all the listed tags
        /// </summary>
        /// <param name="abilitySystem">Ability System</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns><para>true, if the Ability System has all tags</para>
        /// false, if system is invalid or missing 1 or more tags</returns>
        public static bool HasAllTags(this AbilitySystemBehaviour abilitySystem, TagSO[] tags)
        {
            if (abilitySystem == null) return false;
            return abilitySystem.TagSystem.HasAllTags(tags);
        }

        /// <summary>
        /// Checks if an Ability System has none of the listed tags
        /// </summary>
        /// <param name="abilitySystem">Ability System</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns><para>true, if the Ability System has none of the tags</para>
        /// false, if system is invalid or has 1 or more tags</returns>
        public static bool HasNoneTags(this AbilitySystemBehaviour abilitySystem, TagSO[] tags)
        {
            if (abilitySystem == null) return false;
            return !abilitySystem.TagSystem.HasAnyTag(tags);
        }

        public static bool IsSatisfyTagRequirements(this AbilitySystemBehaviour abilitySystem,
            TagRequireIgnoreDetails tagRequirements)
        {
            return abilitySystem.HasAllTags(tagRequirements.RequireTags) &&
                   abilitySystem.HasNoneTags(tagRequirements.IgnoreTags);
        }
    }
}
