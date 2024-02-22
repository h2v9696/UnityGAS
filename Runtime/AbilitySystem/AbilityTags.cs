using System;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AbilitySystem
{
    [Serializable]
    public class AbilityTags
    {
        /// <summary>
        /// Tag to define this ability should be unique
        /// </summary>
        public TagSO AbilityTag;

        /// <summary>
        /// Active the ability on the same system will cancel any ability that have these tags
        /// </summary>
        public TagSO[] CancelAbilityWithTags = Array.Empty<TagSO>();

        /// <summary>
        /// Prevents execution of any other Abilities with a matching Tag while this Ability is executing.
        /// Ability that have these tags will be blocked from activating on the same system
        /// e.g. silencing ability that enemy could use to prevent use to use any ability
        /// </summary>
        public TagSO[] BlockAbilityWithTags = Array.Empty<TagSO>();

        /// <summary>
        /// These tags will be granted to the source system while this ability is active
        /// </summary>
        public TagSO[] ActivationTags = Array.Empty<TagSO>();

        /// <summary>
        /// This ability can only active if owner system has all of the RequiredTags
        /// and none of the Ignore tags
        /// </summary>
        public TagRequireIgnoreDetails OwnerTags = new();

        /// <summary>
        /// This ability can only active if the Source system has all the required tags
        /// and none of the Ignore tags
        /// </summary>
        public TagRequireIgnoreDetails SourceTags = new();

        /// <summary>
        /// This ability can only active if the ALL Targets system has all the required tags
        /// and none of the Ignore tags
        /// </summary>
        public TagRequireIgnoreDetails TargetTags = new();
    }

    [Serializable]
    public class TagRequireIgnoreDetails
    {
        [Tooltip("All of these tags must be present in the ability system")]
        public TagSO[] RequireTags = Array.Empty<TagSO>();
        
        [Tooltip("None of these tags can be present in the ability system")]
        public TagSO[] IgnoreTags = Array.Empty<TagSO>();
    }
}