using System.Collections;
using H2V.GameplayAbilitySystem.EffectSystem;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.EffectSystem.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace H2V.GameplayAbilitySystem.Tests.EffectSystem
{
    [TestFixture]
    public class DurationPolicyTests : EffectTestFixtureBase
    {
        private EffectAttributeModifierTestCase[] _effectModifierTestCases;
        
        [OneTimeSetUp]
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();
            CreateTestCases();
        }

        private void CreateTestCases()
        {
            _effectModifierTestCases = new EffectAttributeModifierTestCase[]
            {
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Add, 10f, 110f),
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Multiply, 1f, 200f),
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Divide, 0.5f, 50f),
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Override, 1f, 1f),
            };
        }

        [UnityTest]
        public IEnumerator ApplyEffect_ShouldModifyCorrectly()
        {
            var effectSystem = _mainSystem.GameplayEffectSystem;
            float duration = .25f;
            foreach (var testCase in _effectModifierTestCases)
            {
                var geDef = new GameplayEffectDefBuilder().
                    WithName("DurationEffect").
                    WithPolicy(new DurationalPolicy(duration)).
                    WithEffectDetails(new() {
                        Modifiers = new[] { testCase.Modifier }
                    }).Build();

                var activeSpec = _mainSystem.ApplyEffectToSelf(geDef);
                
                _mainSystem.AttributeSystem.TryGetAttributeValue(_health, out var health);
                Assert.AreEqual(testCase.ExpectedValue, health.CurrentValue);
                Assert.IsTrue(activeSpec.IsValid());
                Assert.AreEqual(1, effectSystem.AppliedEffects.Count);
                yield return new WaitForSeconds(duration);
                
                Assert.AreEqual(0, effectSystem.AppliedEffects.Count);
                ResetAttributes(_mainSystem);
            }
        }

        [UnityTest]
        [TestCase(true, ExpectedResult = null)]
        [TestCase(false, ExpectedResult = null)]
        public IEnumerator ApplyEffect_WithStack_ShouldExpireCorrectly(bool isResetWhenStackChanged)
        {
            var effectSystem = _mainSystem.GameplayEffectSystem;
            float duration = .25f;
            var testCase = _effectModifierTestCases[0];

            var geDef = new GameplayEffectDefBuilder().
                WithName($"DurationEffectWithStack_Reset{isResetWhenStackChanged}").
                WithPolicy(new DurationalPolicy(duration, isResetWhenStackChanged)).
                WithEffectDetails(new() {
                    Modifiers = new[] { testCase.Modifier }
                }).
                WithStackingDetails(new() {
                    StackingType = EGameplayEffectStackingType.AggregateByTarget,
                    StackLimitCount = 3
                })
                .Build();

            var activeSpec = _mainSystem.ApplyEffectToSelf(geDef);
            
            yield return new WaitForSeconds(duration/2);
            // Increase stack by apply another one
            _mainSystem.ApplyEffectToSelf(geDef);
            
            Assert.AreEqual(2, activeSpec.StackCount);

            yield return new WaitForSeconds(duration/2);

            // If not reset duration when stack changed, the effect should expired
            Assert.AreEqual(!isResetWhenStackChanged, activeSpec.Expired);
            if (!isResetWhenStackChanged) yield break;

            yield return new WaitForSeconds(duration);

            Assert.AreEqual(1, activeSpec.StackCount);
            yield return new WaitForSeconds(duration);
            Assert.AreEqual(0, effectSystem.AppliedEffects.Count);
        }

        [UnityTest]
        public IEnumerator ApplyEffect_WithStack_ShouldModifyCorrectly()
        {
            float duration = .25f;
            var testCase = _effectModifierTestCases[0];
            var geDef = new GameplayEffectDefBuilder().
                WithName("DurationEffect").
                WithPolicy(new DurationalPolicy(duration, true)).
                WithEffectDetails(new() {
                    Modifiers = new[] { testCase.Modifier }
                }).
                WithStackingDetails(new() {
                    StackingType = EGameplayEffectStackingType.AggregateByTarget,
                    StackLimitCount = 3
                }).Build();

            _mainSystem.ApplyEffectToSelf(geDef);

            _mainSystem.AttributeSystem.TryGetAttributeValue(_health, out var hp);
            Assert.AreEqual(testCase.ExpectedValue, hp.CurrentValue);

            yield return new WaitForSeconds(duration/2);

            _mainSystem.ApplyEffectToSelf(geDef);
            _mainSystem.AttributeSystem.TryGetAttributeValue(_health, out hp);
            Assert.AreEqual(testCase.ExpectedValue + 10, hp.CurrentValue);

            yield return new WaitForSeconds(duration);
            yield return null;

            _mainSystem.AttributeSystem.TryGetAttributeValue(_health, out hp);
            Assert.AreEqual(testCase.ExpectedValue, hp.CurrentValue);
        }

        [UnityTest]
        [TestCase(false, false, ExpectedResult = null)]
        [TestCase(false, true, ExpectedResult = null)]
        [TestCase(true, false, ExpectedResult = null)]
        [TestCase(true, true, ExpectedResult = null)]
        public IEnumerator ApplyEffect_StackOverflow_ShouldBehaveCorrectly(
            bool isAllowOverflowApplication, bool isClearStackOnOverflow)
        {
            float duration = .25f;
            var testCase = _effectModifierTestCases[0];
            var overflowGE = new GameplayEffectDefBuilder().WithName("OverflowEffect").
                WithPolicy(new DurationalPolicy(duration, true)).Build();

            var geDef = new GameplayEffectDefBuilder().
                WithName("DurationEffect").
                WithPolicy(new DurationalPolicy(duration, true)).
                WithEffectDetails(new() {
                    Modifiers = new[] { testCase.Modifier }
                }).
                WithStackingDetails(new() {
                    StackingType = EGameplayEffectStackingType.AggregateByTarget,
                    StackLimitCount = 2,
                    IsAllowOverflowApplication = isAllowOverflowApplication,
                    IsClearStackOnOverflow = isClearStackOnOverflow,
                    OverflowEffects = new[] { overflowGE }
                }).Build();

            var activeSpec = _mainSystem.ApplyEffectToSelf(geDef);
            _mainSystem.ApplyEffectToSelf(geDef);
            _mainSystem.ApplyEffectToSelf(geDef);
            // Reach max stack, stack is max and check overflow effect
            yield return new WaitForSeconds(duration / 2);

            var removedEffectCount = !isAllowOverflowApplication && isClearStackOnOverflow ? 1 : 0;

            if (!isClearStackOnOverflow)
                Assert.AreEqual(2, activeSpec.StackCount);

            Assert.AreEqual(2 - removedEffectCount, _mainSystem.GameplayEffectSystem.AppliedEffects.Count);

            yield return null;
            _mainSystem.GameplayEffectSystem.ClearEffects();
        }
    }
}