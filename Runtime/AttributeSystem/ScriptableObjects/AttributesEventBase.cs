using H2V.GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects
{
    public abstract class AttributesEventBase : ScriptableObject
    {
        /// <summary>
        /// Last chance to modify the attribute before it's update
        /// </summary>
        /// <param name="attributeSystem"></param>
        /// <param name="newAttributeValue">modify this</param>
        public abstract void PreAttributeChange(
            AttributeSystemBehaviour attributeSystem,
            ref AttributeValue newAttributeValue
        );

        /// <summary>
        /// Use this for case such as showing a damage number
        /// </summary>
        public abstract void PostAttributeChange(
            AttributeSystemBehaviour attributeSystem,
            ref AttributeValue oldAttributeValue,
            ref AttributeValue newAttributeValue
        );
    }
}