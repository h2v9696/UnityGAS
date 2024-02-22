using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.AttributeSystem.Components;
using NUnit.Framework;
using H2V.GameplayAbilitySystem.AttributeSystem;
using UnityEngine;
using System.Collections.Generic;

namespace H2V.GameplayAbilitySystem.Tests.AttributeSystem
{
    public class AttributeSystemBehaviourTests
    {
        private AttributeSystemBehaviour _attributeSystem;
        private static List<ModifierTestCase> _modifierTestCases = ModifierTestCaseProvider.GetTestCases();

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject();
            _attributeSystem = go.AddComponent<AttributeSystemBehaviour>();
        }

        private AttributeSO SetupAddAttribute()
        {
            var attribute = ScriptableObject.CreateInstance<AttributeSO>();
            _attributeSystem.AddAttribute(attribute);
            return attribute;
        }

        private AttributeSO SetupAddAttributeAndBaseValue(float value)
        {
            var attribute = SetupAddAttribute();
            _attributeSystem.SetAttributeBaseValue(attribute, value);
            return attribute;
        }

        [Test]
        public void Init_IsEmptyValues()
        {
            _attributeSystem.Init();

            Assert.AreEqual(0, _attributeSystem.AttributeValues.Count);
        }

        [Test]
        public void AddAttribute_IsAdded_HasAttributeValue()
        {
            var attribute = SetupAddAttribute();

            Assert.AreEqual(1, _attributeSystem.AttributeValues.Count);
            Assert.AreEqual(attribute, _attributeSystem.AttributeValues[0].Attribute);
        }

        [Test]
        public void AddAttribute_SameAttributeAdded_OnlyHasOneAttribute()
        {
            var attribute = SetupAddAttribute();
            _attributeSystem.AddAttribute(attribute);

            Assert.AreEqual(1, _attributeSystem.AttributeValues.Count);
        }

        [Test]
        public void GetAttributeIndexCache_CorrectCached()
        {
            var attribute = SetupAddAttribute();

            var cache = _attributeSystem.GetAttributeIndexCache();
            Assert.AreEqual(1, cache.Count);
            Assert.AreEqual(0, cache[attribute]);
        }

        [Test]
        public void HasAttribute_AttributeAdded_ReturnTrue()
        {
            var attribute = SetupAddAttribute();

            bool hasAttribute = _attributeSystem.HasAttribute(attribute, out var value);
            Assert.IsTrue(hasAttribute);
            Assert.AreEqual(attribute, value.Attribute);
        }

        [Test]
        public void HasAttribute_AttributeNotAdded_ReturnFalse()
        {
            var attribute = ScriptableObject.CreateInstance<AttributeSO>();

            bool hasAttribute = _attributeSystem.HasAttribute(attribute, out var value);
            Assert.IsFalse(hasAttribute);
            Assert.IsNull(value.Attribute);
        }

        [Test]
        public void TryGetAttributeValue_GetCorrectAddedAttribute()
        {
            var baseValue = 10f;
            var attribute = SetupAddAttributeAndBaseValue(baseValue);

            bool hasAttribute = _attributeSystem.TryGetAttributeValue(attribute, out var value);
            Assert.IsTrue(hasAttribute);
            Assert.AreEqual(attribute, value.Attribute);
            Assert.AreEqual(baseValue, value.BaseValue);
        }

        [Test]
        [TestCase(-10f)]
        [TestCase(0f)]
        [TestCase(10f)]
        public void SetAttributeBaseValue_BaseValueCorrect(float baseValue)
        {
            var attribute = SetupAddAttributeAndBaseValue(baseValue);
            _attributeSystem.TryGetAttributeValue(attribute, out var value);

            Assert.AreEqual(baseValue, value.BaseValue);
        }

        [Test]
        public void SetAttributeValue_EqualNewValue()
        {
            var baseValue = 10f;
            var attribute = SetupAddAttribute();
            Assert.AreEqual(0, _attributeSystem.AttributeValues[0].BaseValue);
            var newAttributeValue = new AttributeValue(attribute);
            newAttributeValue.BaseValue = baseValue;

            _attributeSystem.SetAttributeValue(attribute, newAttributeValue);
            Assert.AreEqual(baseValue, _attributeSystem.AttributeValues[0].BaseValue);
        }

        [Test]
        public void TryAddModifierToAttribute_ValueChanged(
            [ValueSource(nameof(_modifierTestCases))] ModifierTestCase testCase)
        {
            var attribute = SetupAddAttributeAndBaseValue(testCase.BaseValue);

            foreach (var modifier in testCase.Modifiers)
            {
                _attributeSystem.TryAddModifierToAttribute(modifier.Modifier, attribute, modifier.ModifierType);
            }

            _attributeSystem.UpdateAttributeValues();
            _attributeSystem.TryGetAttributeValue(attribute, out var value);
            Assert.AreEqual(testCase.ExpectedValue, value.CurrentValue);
        }

        [Test]
        public void ResetAllAttributes_AllAttributeValuesReset()
        {
            TryAddModifierToAttribute_ValueChanged(_modifierTestCases[0]);
            TryAddModifierToAttribute_ValueChanged(_modifierTestCases[0]);

            _attributeSystem.ResetAllAttributes();

            foreach (var attributeValue in _attributeSystem.AttributeValues)
            {
                Assert.AreEqual(0, attributeValue.CurrentValue);
            }
        }

        [Test]
        public void ResetAttributeModifiers_AllAttributeValuesResetToBase(
            [ValueSource(nameof(_modifierTestCases))] ModifierTestCase testCase)
        {
            TryAddModifierToAttribute_ValueChanged(testCase);
            TryAddModifierToAttribute_ValueChanged(testCase);

            _attributeSystem.ResetAttributeModifiers();
            _attributeSystem.UpdateAttributeValues();

            foreach (var attributeValue in _attributeSystem.AttributeValues)
            {
                Assert.AreEqual(testCase.BaseValue, attributeValue.CurrentValue);
            }
        }
    }
}