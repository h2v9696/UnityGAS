using System;
using System.Collections.Generic;
using H2V.GameplayAbilitySystem.AttributeSystem;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects
{
    public enum EGameplayEffectCaptureSource
    {
        [Tooltip("Where the effect created from")]
        Source,

        [Tooltip("here the effect will be applied to")]
        Target
    }

    /// <summary>
    /// Use this to quickly setup a capture def for attribute and where to capture it from.
    /// </summary>
    [Serializable]
    public struct CustomExecutionAttributeCaptureDef
    {
        public AttributeSO Attribute;
        public EGameplayEffectCaptureSource CaptureFrom;
    }

    public struct CustomExecutionParameters
    {
        public AbilitySystemComponent TargetSystem;
        public AbilitySystemComponent SourceSystem;
        public GameplayEffectSpec EffectSpec;

        public CustomExecutionParameters(GameplayEffectSpec effectSpec)
        {
            EffectSpec = effectSpec;
            TargetSystem = effectSpec.Target;
            SourceSystem = effectSpec.Source;
        }

        /// <summary>
        /// Get the attribute value from the defined source
        /// 
        /// TODO: Should calculate the magnitude
        /// </summary>
        /// <param name="captureAttributeDef"></param>
        /// <param name="attributeValue"></param>
        /// <param name="defaultCurrentValue">When <see cref="AttributeValue.CurrentValue"/> == 0, get the default</param>
        public readonly void TryGetAttributeValue(CustomExecutionAttributeCaptureDef captureAttributeDef,
            out AttributeValue attributeValue, float defaultCurrentValue = 0f)
        {
            attributeValue = new AttributeValue();
            switch (captureAttributeDef.CaptureFrom)
            {
                case EGameplayEffectCaptureSource.Source:
                    SourceSystem.AttributeSystem.TryGetAttributeValue(captureAttributeDef.Attribute,
                        out attributeValue);
                    break;
                case EGameplayEffectCaptureSource.Target:
                    TargetSystem.AttributeSystem.TryGetAttributeValue(captureAttributeDef.Attribute,
                        out attributeValue);
                    break;
            }

            if (attributeValue.CurrentValue == 0f && defaultCurrentValue != 0f)
            {
                attributeValue.CurrentValue = defaultCurrentValue;
            }
        }
    }

    public class GameplayEffectCustomExecutionOutput
    {
        public readonly List<ModifierEvaluatedData> Modifiers = new();

        public void Add(ModifierEvaluatedData modifier) => Modifiers.Add(modifier);
    }

    /// <summary>
    /// Override this to create custom logic for calculating the effect modifiers before it is applied to the target.
    ///
    /// Modifiers will be apply by <see cref="ActiveGameplayEffect"/>
    /// </summary>
    public abstract class EffectExecutionSO : ScriptableObject
    {
        /// <summary>
        /// Custom logic for calculating the effect modifier before it is applied to the target.
        /// such as calculate the damage based on the target's defense and owner's attack damage.
        /// by default this will do nothing
        /// 
        /// For case attack that depends on the source damage and target defends
        /// this would add a new -HP modifier with Add type to the <see cref="outModifiers"/>
        /// </summary>
        /// <param name="executionParams"></param>
        /// <param name="outModifiers">List of modifier that calculation added</param>
        public abstract void Execute(ref CustomExecutionParameters executionParams,
            ref GameplayEffectCustomExecutionOutput outModifiers);
    }
}