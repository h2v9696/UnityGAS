using System;
using System.Collections.Generic;
using H2V.ExtensionsCore.Helpers;
using H2V.GameplayAbilitySystem.AttributeSystem;
using H2V.GameplayAbilitySystem.AttributeSystem.Components;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    /// <summary>
    /// Represent an effect that being active on the system
    /// This is a wrapper for <see cref="GameplayEffectSpec"/> to store the computed modifier
    /// - What GameplayEffectSpec
    /// - Start Time
    /// - When To execute next (periodic instant effect)
    /// </summary>
    [Serializable]
    public class ActiveGameplayEffect
    {
        private GameplayEffectSpec _spec;
        private AttributeSystemBehaviour TargetAttributeSystem => _spec.Target.AttributeSystem;
        public GameplayEffectSpec Spec => _spec;
        public int StackCount => _spec.StackCount;
        public int StackLimitCount => _spec.StackingDetails.StackLimitCount;

        /// <summary>
        /// Calculated modifiers after <see cref="EffectExecutionSO.Execute"/>
        /// </summary>
        private List<ModifierEvaluatedData> _computedModifiers = new();
        public List<ModifierEvaluatedData> ComputedModifiers => _computedModifiers;

        public EModifierType ModifierType => _spec.EffectDefDetails.StackingType;
        public bool IsActive { get; set; } = true;
        public bool Expired => _spec == null || _spec.IsExpired || IsActive == false;

        public TagSO EffectTag => _spec.EffectTag;


        /// <summary>
        /// To prevent null reference exception
        /// </summary>
        public ActiveGameplayEffect() { }

        /// <summary>
        /// Currently only support capture the modifier when the effect is created
        ///
        /// For case such as periodic effect with up to date modifier, we need to update the modifier
        /// </summary>
        /// <param name="spec"></param>
        public ActiveGameplayEffect(GameplayEffectSpec spec)
        {
            _spec = spec;
            ExecuteCustomCalculations(_spec, out _computedModifiers);
        }

        private void ExecuteCustomCalculations(GameplayEffectSpec effectSpec,
            out List<ModifierEvaluatedData> evaluatedDatas)
        {
            var customCalculations = effectSpec.CustomExecutions;
            GameplayEffectCustomExecutionOutput output = new();
            for (int index = 0; index < customCalculations.Length; index++)
            {
                var customCalculation = customCalculations[index];
                if (customCalculation == null) continue;

                var executionParams = new CustomExecutionParameters(effectSpec);
                customCalculation.Execute(ref executionParams, ref output);
            }

            evaluatedDatas = output.Modifiers;
        }

        public virtual void Update(float deltaTime) { }
        
        public virtual bool CanRemoveFrom(EffectSystemBehaviour fromSystem)
            => true;

        /// <summary>
        /// I added OnRemoved because destructor is not working in Unity
        /// </summary>
        public virtual void OnRemovedFrom(EffectSystemBehaviour fromSystem)
        {
            RemoveCustomEffectFrom(fromSystem.Owner);
        }

        public bool IsValid()
            => _spec != null && _spec.IsValid() && _spec.IsExpired == false && IsActive;

        public void UpdateStackCount(int newStackCount)
        {
            var oldStackCount = _spec.StackCount;

            if (StackLimitCount > 0)
            {
                newStackCount = Math.Min(newStackCount, StackLimitCount);
            }
            _spec.StackCount = newStackCount;
            OnSpecStackChanged(oldStackCount, newStackCount);
        }

        /// <summary>
        /// Call back when stack changes, derived for custom logic when stack changed
        /// </summary>
        /// <param name="otherSpec"></param>
        protected virtual void OnSpecStackChanged(int oldStackCount, int newStackCount)
        {
        }

        /// <summary>
        /// How is this active effect modify the target system
        /// - Infinite and Duration effect would add modifier to the system
        /// - Instant effect would modify the base value of the attribute
        /// - Periodic will check if the period interval and apply the effect just like instant effect
        /// </summary>
        public virtual void ExecuteActiveEffect()
        {
            if (IsActive == false) return;
            ApplyModifiersUsingEffectDefDetails();
            ApplyModifiersUsingComputedExecCal();
        }

        /// <summary>
        /// Try active effect on soure by itself
        /// without applying modifier or adding tag... because these effect only applied instantly
        /// Or stack effect only applied at first time and only update stack count when apply again
        /// </summary>
        /// <returns></returns>
        public virtual bool TrySelfActiveEffect()
        {
            return false;
        }

        private void ApplyModifiersUsingEffectDefDetails()
        {
            // apply modifier using def first
            for (var index = 0; index < _spec.EffectDef.EffectDetails.Modifiers.Length; index++)
            {
                var modifier = _spec.EffectDef.EffectDetails.Modifiers[index];
                AddModifierToAttribute(modifier.Attribute, modifier.OperationType, _spec.GetModifierMagnitude(index));
            }
        }

        private void ApplyModifiersUsingComputedExecCal()
        {
            // apply modifier after execute custom calculation
            foreach (var modifier in _computedModifiers)
                AddModifierToAttribute(modifier.Attribute, modifier.OpType, modifier.Magnitude);
        }

        private void AddModifierToAttribute(AttributeSO attribute, EAttributeModifierOperationType opType, float magnitude)
        {
            var modToApply = new Modifier();
            switch (opType)
            {
                case EAttributeModifierOperationType.Add:
                    modToApply.Additive = magnitude;
                    break;
                case EAttributeModifierOperationType.Multiply:
                    modToApply.Multiplicative = magnitude;
                    break;
                case EAttributeModifierOperationType.Divide:
                    modToApply.Multiplicative = -magnitude;
                    break;
                case EAttributeModifierOperationType.Override:
                    modToApply.Overriding = magnitude;
                    break;
            }

            _spec.Target.AttributeSystem.TryAddModifierToAttribute(modToApply, attribute, ModifierType);
        }

        /// <summary>
        /// The case is we have an effect with modifier want to affect an attribute that is not in the system yet
        ///
        /// e.g. Modifier to increase gold drop rate, but the attribute system does not have gold drop rate attribute.
        /// We can either add the attribute to the system or this method would add it for us. only at runtime
        /// </summary>
        private void AddAttributeToSystemIfNotExists(AttributeSO attribute)
        {
            if (!TargetAttributeSystem.HasAttribute(attribute, out _))
                TargetAttributeSystem.AddAttribute(attribute);
        }

        protected bool InternalExecuteMod(ModifierEvaluatedData modEvalData)
        {
            ModifierCallbackData executeData = new(Spec, modEvalData, Spec.Source);
            var targetSystem = Spec.Target.GameplayEffectSystem;

            /*
             * This should apply 'gamewide' rules. Such as clamping Health to MaxHealth or granting +3 health
             * for every point of strength, etc
             */
            if (targetSystem.PreGameplayEffectExecute(executeData) == false) return false;

            ModifyAttributeBaseValue(modEvalData.Attribute, modEvalData.OpType, modEvalData.Magnitude, ref executeData);

            targetSystem.PostGameplayEffectExecute(executeData);

            return true;
        }

        public void ModifyAttributeBaseValue(AttributeSO attribute, EAttributeModifierOperationType modifierOp,
            float magnitude, ref ModifierCallbackData executeData)
        {
            var targetAttributeSystem = Spec.Target.AttributeSystem;
            if (!targetAttributeSystem.TryGetAttributeValue(attribute, out var curBase)) return;
            float newBase = StaticExecModOnBaseValue(curBase.BaseValue, modifierOp, magnitude);
            targetAttributeSystem.SetAttributeBaseValue(attribute, newBase);

            Debug.Log($"ActiveGameplayEffect::ModifyBaseAttribute [{attribute.name}]" +
                $" old base value: [{curBase.BaseValue}] " + $" new base value: [{newBase}]");
        }

        private static float StaticExecModOnBaseValue(float baseValue, EAttributeModifierOperationType opType, float magnitude)
        {
            switch (opType)
            {
                case EAttributeModifierOperationType.Add:
                    return baseValue + magnitude;
                case EAttributeModifierOperationType.Multiply:
                    return baseValue * magnitude;
                case EAttributeModifierOperationType.Divide:
                    if (magnitude.NearlyEqual(0) == false)
                        return baseValue / magnitude;
                    break;
                case EAttributeModifierOperationType.Override:
                    return magnitude;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opType), opType, null);
            }

            return baseValue;
        }

        /// <summary>
        /// For example: remove other effect or grant ability on applying this effect
        /// Only execute if the effect is stay on the target system and has destruction when effect removed
        /// Eg. Remove granted ability when this effect is removed
        /// </summary>
        /// <param name="system"></param>
        public virtual void ExecuteCustomEffectOnApplied(AbilitySystemComponent system)
        {
            var addditionEffects = _spec.EffectDef.AdditionApplyEffects;
            foreach (var additionEffect in addditionEffects)
            {
                additionEffect.OnEffectSpecApplied(system);
            }
        }

        protected virtual void RemoveCustomEffectFrom(AbilitySystemComponent system)
        {
            var addditionEffects = _spec.EffectDef.AdditionApplyEffects;
            foreach (var additionEffect in addditionEffects)
            {
                additionEffect.OnEffectSpecRemoved(system);
            }
        }
    }
}