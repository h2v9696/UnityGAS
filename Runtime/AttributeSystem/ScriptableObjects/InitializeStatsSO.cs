using System;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InitStats", menuName = "H2V/Gameplay Ability System/Attributes/Initialize Stats")]
    public class InitializeStatsSO : ScriptableObject
    {
        [field: SerializeField]
        public AttributeWithValue[] Stats { get; private set; }
    }

    [Serializable]
    public struct AttributeWithValue
    {
        public AttributeWithValue(AttributeSO attribute, float value)
        {
            Attribute = attribute;
            Value = value;
        }

        public AttributeSO Attribute;
        public float Value;
    }
}