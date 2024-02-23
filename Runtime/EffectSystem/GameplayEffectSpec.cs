using System;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem.Utilities;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    /// <summary>
    /// <para>GameplayEffect.h/cpp</para>
    /// A specification represent an effect that created from <see cref="IGameplayEffectDef"/>
    /// When the effect is applied to the system, it will create a new <see cref="ActiveGameplayEffect"/> to store the computed modifier
    /// Every effect should create from <see cref="IGameplayEffectDef.CreateEffectSpec"/>
    /// </summary>
    [Serializable]
    public class GameplayEffectSpec
    {
        /// <summary>
        /// The component give/apply this effect
        /// </summary>
        public AbilitySystemComponent Source { get; set; }

        /// <summary>
        /// The system this effect applying to. Can be the same as <see cref="Source"/> and it will be self apply effect
        /// </summary>
        public AbilitySystemComponent Target { get; set; }

        public EffectDetails EffectDefDetails { get; set; }
        public EffectExecutionSO[] CustomExecutions { get; set; }

        public bool IsExpired { get; set; }

        /// <summary>
        /// Calculated modifiers for this effect
        /// </summary>
        public ModifierSpec[] Modifiers { get; set; } = Array.Empty<ModifierSpec>();

        public int StackCount { get; set; }

        public GameplayEffectContextHandle Context { get; set; }

        /// <summary>
        /// Which Data/SO the effect is based on
        /// </summary>
        [field: SerializeField] public IGameplayEffectDef EffectDef { get; private set; }
        public TagSO EffectTag => EffectDef.EffectTag;

        public StackingDetails StackingDetails => EffectDef.StackingDetails;

        public void InitEffect(IGameplayEffectDef effectDef, AbilitySystemComponent source)
        {
            Source = source;

            if (effectDef == null)
            {
                Debug.LogWarning("EffectSO is null");
                return;
            }

            EffectDef = effectDef;
            EffectDefDetails = EffectDef.EffectDetails;
            CustomExecutions = EffectDef.CustomExecutions;

            Modifiers = new ModifierSpec[EffectDef.EffectDetails.Modifiers.Length];
            foreach (var modifier in EffectDef.EffectDetails.Modifiers)
            {
                if (modifier.ModifierMagnitude == null) continue;
                modifier.ModifierMagnitude.Initialize(this);
            }

            CalculateModifierMagnitudes();


            OnInitEffect(effectDef, source);
        }

        public virtual void OnInitEffect(IGameplayEffectDef effectDef, AbilitySystemComponent source) { }

        public bool CanApply()
        {
            if (!IsValid()) return false;

            for (var index = 0; index < EffectDef.ApplicationConditions.Length; index++)
            {
                var condition = EffectDef.ApplicationConditions[index];
                if (condition == null || condition.IsPass(this)) continue;
                Debug.Log(@$"GameplayEffectSpec::CanApply::False {condition}
                    doesn't meet the requirement for {EffectDef.Name}");
                return false;
            }

            return true;
        }

        public void CalculateModifierMagnitudes()
        {
            var effectSODetails = EffectDef.EffectDetails;
            for (var index = 0; index < effectSODetails.Modifiers.Length; index++)
            {
                var modifierDef = effectSODetails.Modifiers[index];
                var modifierSpec = Modifiers[index];
                modifierSpec.ModifierOperation = modifierDef.OperationType;

                var modifierMagnitude = modifierDef.ModifierMagnitude;
                if (modifierMagnitude == null)
                {
                    modifierSpec.EvaluatedMagnitude = modifierDef.Value;
                    Modifiers[index] = modifierSpec;
                    continue;
                }

                if (!modifierMagnitude.TryCalculateMagnitude(this, ref modifierSpec.EvaluatedMagnitude))
                {
                    modifierSpec.EvaluatedMagnitude = 0;
                    Debug.Log(@$"Modifier on spec {EffectDef.Name}
                        failed to calculate magnitudeFalling back to 0.");
                }


                Modifiers[index] = modifierSpec;
            }
        }

        public bool IsValid() 
        {
            if (EffectDef == null)
            {
                Debug.LogWarning("GameplayEffectSpec::IsValid:: No EffectDef.");
                return false;
            }

            if (Source == null) return false;

            var effectDetails = EffectDef.EffectDetails;
            for (var index = 0; index < effectDetails.Modifiers.Length; index++)
            {
                var modifier = effectDetails.Modifiers[index];
                if (!modifier.Attribute)
                {
                    Debug.LogWarning(
                        $"GameplayEffectSpec::CanApply:: Effect {EffectDef.Name} has a modifier with no Attribute at idx[{index}].");
                    return false;
                }
            }

            return true;
        }


        public ActiveGameplayEffect CreateActiveEffectSpec()
            => EffectDef.Policy.CreateActiveEffect(this);

        public float GetModifierMagnitude(int modifierIdx)
        {
            var singleEvaluatedMagnitude = Modifiers[modifierIdx].EvaluatedMagnitude;
            if (StackingDetails.IsStack())
            {
                singleEvaluatedMagnitude = GameplayEffectUtilities.ComputeStackedModifierMagnitude(
                    singleEvaluatedMagnitude, StackCount, Modifiers[modifierIdx].ModifierOperation);
            }

            return singleEvaluatedMagnitude;
        }

        public bool IsStackableWith(GameplayEffectSpec otherSpec)
        {
            if (EffectDef != otherSpec.EffectDef) return false;

            if (StackingDetails.StackingType == EGameplayEffectStackingType.AggregateByTarget)
                return true;

            if (Source != null && Source == otherSpec.Context.GetContext().InstigatorAbilitySystem)
                return true;

            return true;
        }
    }

    /// <summary>
    /// Wrapper for <see cref="IGameplayEffectDef.EffectDetails.Modifiers"/>
    /// </summary>
    [Serializable]
    public struct ModifierSpec
    {
        public float EvaluatedMagnitude;
        public EAttributeModifierOperationType ModifierOperation;

        private GameplayEffectSpec _effectSpec;
        private ActiveGameplayEffect _activeGameplayEffect;

        public readonly float GetEvaluatedMagnitude() => EvaluatedMagnitude;
    }
}