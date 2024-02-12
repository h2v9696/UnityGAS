using System;
using H2V.GameplayAbilitySystem.AttributeSystem.Components;

namespace H2V.GameplayAbilitySystem.AttributeSystem
{
    public interface IAttributeValueCalculator
    {
        AttributeValue Calculate(AttributeValue baseValue, 
            AttributeSystemBehaviour attributeSystem);
    }

    /// <summary>
    /// Called by <see cref="AttributeSystemBehaviour.InitializeAttributeValues"/> 
    /// to calculate the initial value of the attribute.
    /// There might be a hidden bug here when the current attribute value depends on other attributes.
    /// </summary>
    [Serializable]
    public class InitialAttributeValueCalculator : IAttributeValueCalculator
    {
        public AttributeValue Calculate(AttributeValue baseValue, 
            AttributeSystemBehaviour attributeSystem) => baseValue;
    }

    /// <summary>
    /// Called by <see cref="AttributeSystemBehaviour.UpdateAttributeValues"/> 
    /// to calculate the current value of the attribute.
    /// Return a new <see cref="AttributeValue"/> with the current value set.
    /// Wrap the base value with core modifier first <a herf="https://www.youtube.com/watch?v=JgSvuSaXs3E">source</a> 
    /// before applying external modifier.
    /// This is a basic RPG attribute system implementation you can implement new logic as your need 
    /// </summary>
    [Serializable]
    public class CurrentAttributeValueCalculator : IAttributeValueCalculator
    {
        public AttributeValue Calculate(AttributeValue baseValue, 
            AttributeSystemBehaviour attributeSystem) => CalculateCurrentAttributeValue(baseValue);

        public static AttributeValue CalculateCurrentAttributeValue(AttributeValue attributeValue)
        {
            if (attributeValue.ExternalModifier.Overriding != 0)
            {
                attributeValue.CurrentValue = attributeValue.ExternalModifier.Overriding;
                return attributeValue;
            }

            // order matters here, we want to override core with external
            if (attributeValue.CoreModifier.Overriding != 0)
            {
                attributeValue.CurrentValue = attributeValue.CoreModifier.Overriding;
                return attributeValue;
            }

            attributeValue.CurrentValue = (attributeValue.GetCoreValue() + attributeValue.ExternalModifier.Additive) *
                (attributeValue.ExternalModifier.Multiplicative + 1);
            return attributeValue;
        }
    }

    
    public static class AttributeValueExtensions
    {
        public static float GetCoreValue(this AttributeValue attributeValue)
        {
            if (attributeValue.CoreModifier.Overriding != 0)
                return attributeValue.CoreModifier.Overriding;

            return (attributeValue.BaseValue + attributeValue.CoreModifier.Additive) *
                (attributeValue.CoreModifier.Multiplicative + 1);
        }
    }
}