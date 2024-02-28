using NUnit.Framework;
using H2V.GameplayAbilitySystem.Components;
using UnityEngine;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.EffectSystem;
using UnityEngine.TestTools;
using System.Collections;
using H2V.GameplayAbilitySystem.EffectSystem.Components;

namespace H2V.GameplayAbilitySystem.Tests.EffectSystem
{
    public class EffectTestFixtureBase
    {
        public struct EffectAttributeModifierTestCase
        {
            public EffectAttributeModifier Modifier;
            public float ExpectedValue;
        }

        protected AbilitySystemComponent _mainSystem;
        protected AbilitySystemComponent _targetSystem;

        protected AttributeSO _health;
        protected AttributeSO _attack;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            _health = ScriptableObject.CreateInstance<AttributeSO>();
            _attack = ScriptableObject.CreateInstance<AttributeSO>();
            _mainSystem = CreateAbilitySystem();
            _targetSystem = CreateAbilitySystem();
            _mainSystem.Init();
            _targetSystem.Init();
            ResetAttributes(_mainSystem);
            ResetAttributes(_targetSystem);
        }

        internal AbilitySystemComponent CreateAbilitySystem()
        {
            var go = new GameObject();
            var asc = go.AddComponent<AbilitySystemComponent>();
            return asc;
        }

        protected void ResetAttributes(AbilitySystemComponent systemComponent)
        {
            var attributeSystem = systemComponent.AttributeSystem;
            attributeSystem.SetAttributeBaseValue(_health, 100);
            attributeSystem.SetAttributeBaseValue(_attack, 10);
        }

        protected EffectAttributeModifierTestCase CreateEffectModifierTestCase(AttributeSO attribute, 
            EAttributeModifierOperationType operationType,
            float value, float expectedValue)
        {
            return new()
            {
                Modifier = new EffectAttributeModifier()
                {
                    Attribute = attribute,
                    OperationType = operationType,
                    Value = value
                },
                ExpectedValue = expectedValue
            };
        }
    }
}