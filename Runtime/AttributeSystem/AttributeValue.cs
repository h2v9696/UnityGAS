using System;
using H2V.GameplayAbilitySystem.AttributeSystem.Components;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AttributeSystem
{
    public enum EModifierType
    {
        External,
        Core,
    }

    /// <summary>
    /// Represent a value of a <see cref="AttributeScriptableObject"/>
    /// </summary>
    [Serializable]
    public struct AttributeValue
    {
        [field: SerializeField] public AttributeSO Attribute { get; set; }
        [field: SerializeField, Tooltip("This is permanent value, from stats, gear, etc.")]
        public float BaseValue { get; set; }
        [field: SerializeField, Tooltip("This is temporary value, modify from effects and might be removed")]
        public float CurrentValue { get; set; }

        /// <summary>
        /// Sum of all external effects
        /// For ability/effect external stats
        /// This is for external modifier such as temporary buff, in combat buff
        /// </summary>
        public Modifier ExternalModifier;

        /// <summary>
        /// Based on For Honor GDC talk which will cause wrong calculation
        /// This is for Gameplay Difficulty multiplier, Gear, Permanent Buffs and passive
        /// <seealso herf="https://www.youtube.com/watch?v=JgSvuSaXs3E"/>
        /// </summary>
        public Modifier CoreModifier;

        public AttributeValue(AttributeSO attribute)
        {
            Attribute = attribute;
            BaseValue = CurrentValue = 0f;
            ExternalModifier = new Modifier();
            CoreModifier = new Modifier();
        }

        public AttributeValue Clone()
        {
            return new AttributeValue()
            {
                CurrentValue = CurrentValue,
                BaseValue = BaseValue,
                ExternalModifier = ExternalModifier,
                CoreModifier = CoreModifier,
                Attribute = Attribute
            };
        }

        public readonly AttributeValue CalculateInitialValue(AttributeSystemBehaviour attributeSystem)
            => Attribute.InitialValueCalculator.Calculate(this, attributeSystem);

        public readonly AttributeValue CalculateCurrentValue(AttributeSystemBehaviour attributeSystem)
            => Attribute.CurrentValueCalculator.Calculate(this, attributeSystem);
    }
}