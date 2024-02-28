using H2V.GameplayAbilitySystem.EffectSystem;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.EffectSystem.Utilities;
using NUnit.Framework;

namespace H2V.GameplayAbilitySystem.Tests.EffectSystem
{
    [TestFixture]
    public class InstantPolicyTests : EffectTestFixtureBase
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
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Multiply, 2f, 200f),
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Divide, 2f, 50f),
                CreateEffectModifierTestCase(_health, EAttributeModifierOperationType.Override, 1f, 1f),
            };
        }

        [Test]
        public void ApplyEffect_ShouldModifyCorrectly()
        {
            foreach (var testCase in _effectModifierTestCases)
            {
                var geDef = new GameplayEffectDefBuilder().
                    WithPolicy(new InstantPolicy()).
                    WithEffectDetails(new() {
                        Modifiers = new[] { testCase.Modifier }
                    }).Build();

                _mainSystem.ApplyEffectToSelf(geDef);
                
                _mainSystem.AttributeSystem.TryGetAttributeValue(_health, out var health);
                Assert.AreEqual(testCase.ExpectedValue, health.CurrentValue);
                ResetAttributes(_mainSystem);
            }
        }
    }
}