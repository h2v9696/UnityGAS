using System;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects;
using H2V.GameplayAbilitySystem.EffectSystem.EffectConditions;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "H2V/Gameplay Ability System/Effects/Gameplay Effect SO")]
    public class GameplayEffectSO : ScriptableObject, IGameplayEffectDef
    {
        public string Name => name;

        [field: SerializeField, TextArea(0, 3)]
        public string Description { get; private set; }

        [field: SerializeField, Tooltip("Tag to define this effect")]
        public TagSO EffectTag { get; private set; }

        /// <summary>
        /// How this effect will be applied to target
        /// <see cref="DurationalPolicy"/> <see cref="InstantPolicy"/>
        /// <see cref="InfinitePolicy"/> <see cref="PeriodicPolicy"/>
        /// </summary>
        [field: SerializeReference, SubclassSelector]
        public IGameplayEffectPolicy Policy { get; private set; } = new InstantPolicy();

        [field: SerializeField, Tooltip("What attribute to affect and how it affected")]
        public EffectDetails EffectDetails { get; private set; } = new();

        [field: SerializeField]
        public StackingDetails StackingDetails { get; private set; }

        [field: SerializeReference, SubclassSelector, Tooltip("Addition effect when this effect is applied on the target")]
        public IAdditionApplyEffect[] AdditionApplyEffects { get; private set; } = Array.Empty<IAdditionApplyEffect>();

        /// <summary>
        /// e.g. Need to calculate based on caster's attribute
        /// </summary>
        [field: SerializeField, Tooltip("How the effect being execute with custom logic. Can leave it null.")]
        public EffectExecutionSO[] CustomExecutions { get; private set; } = Array.Empty<EffectExecutionSO>();

        [field: SerializeReference, SubclassSelector, Tooltip("Custom requirement to know if we can apply effect or not")]
        public IEffectCondition[] ApplicationConditions { get; private set; } = Array.Empty<IEffectCondition>();

        /// <summary>
        /// Create a new Specification from this Definition
        /// </summary>
        /// <param name="ownerSystem">from owner</param>
        /// <returns></returns>
        public GameplayEffectSpec CreateEffectSpec(AbilitySystemComponent ownerSystem,
            GameplayEffectContextHandle context)
        {
            var effect = CreateEffect();
            effect.Context = context;
            effect.InitEffect(this, ownerSystem);
            return effect;
        }

        protected virtual GameplayEffectSpec CreateEffect() => new();
    }
}