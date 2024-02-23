using System;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    /// <summary>
    /// Define how the effect will modify an attribute
    /// </summary>
    [Serializable]
    public struct EffectAttributeModifier
    {
        [Tooltip("Select which attribute will be affected by this modifier")]
        public AttributeSO Attribute;

        [Tooltip("Select which type of modifier")]
        public EAttributeModifierOperationType OperationType;

        /// <summary>
        /// Magnitude of this modifier
        /// </summary>
        [Tooltip("How the Magnitude of this modifier will be calculated")]
        public ModifierComputationSO ModifierMagnitude;

        [Tooltip("Effect value")]
        public float Value;

        public EffectAttributeModifier Clone()
        {
            return new EffectAttributeModifier
            {
                Attribute = Attribute,
                OperationType = OperationType,
                ModifierMagnitude = ModifierMagnitude,
                Value = Value
            };
        }
    }

    public enum EAttributeModifierOperationType
    {
        Add = 0,
        [Tooltip(@"Note that when apply directly to the base value (InstantPolicy), the base will be multiplied by the magnitude.
        But when applied by modifier it will calculate by percent. Eg. Value is 2, Base is * 2, Modifier is + 200%")]
        Multiply = 1,
        [Tooltip("Same as Multiply but the base will be divided by the magnitude")]
        Divide = 2,
        Override = 3
    }
}