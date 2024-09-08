using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects;
using H2V.GameplayAbilitySystem.EffectSystem.EffectConditions;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    public interface IGameplayEffectDef
    {
        string Name { get; }

        TagSO EffectTag { get; }

        /// <summary>
        /// How this effect will be applied to target
        /// <see cref="DurationalPolicy"/> <see cref="InstantPolicy"/>
        /// <see cref="InfinitePolicy"/> <see cref="PeriodicPolicy"/>
        /// </summary>
        IGameplayEffectPolicy Policy { get; }

        EffectDetails EffectDetails { get; }

        StackingDetails StackingDetails { get; }

        IAdditionApplyEffect[] AdditionApplyEffects { get; }

        /// <summary>
        /// e.g. Need to calculate based on caster's attribute
        /// </summary>
        EffectExecutionSO[] CustomExecutions { get; }

        IEffectCondition[] ApplicationConditions { get; }

        /// <summary>
        /// Create a new Specification from this Definition
        /// </summary>
        /// <param name="ownerSystem">from owner</param>
        /// <returns></returns>
        GameplayEffectSpec CreateEffectSpec(AbilitySystemComponent ownerSystem,
            GameplayEffectContextHandle context);
    }

    public struct StackingDetails
    {
        public EGameplayEffectStackingType StackingType;
        public int StackLimitCount;
        public IGameplayEffectDef[] OverflowEffects;
        public bool IsAllowOverflowApplication;
        public bool IsClearStackOnOverflow;

        public readonly bool IsStack()
            => StackingType != EGameplayEffectStackingType.None;
    }

    public enum EGameplayEffectStackingType
    {
        [Tooltip("No stacking. Multiple applications of this GameplayEffect are treated as separate instances.")]
        None = 0,
        [Tooltip("Each caster has its own stack.")]
        AggregateBySource,
        [Tooltip("Each target has its own stack.")]
        AggregateByTarget
    }
}