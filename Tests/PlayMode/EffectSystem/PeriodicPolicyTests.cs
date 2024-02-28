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
    public class PeriodicPolicyTests : EffectTestFixtureBase
    {
        [UnityTest]
        public IEnumerator ApplyEffect_ShouldTickEffect()
        {
            int activeTimes = 3;
            float interval = .25f;

            var tickGE = new GameplayEffectDefBuilder().WithName("TickEffect").
                WithPolicy(new InstantPolicy()).
                WithEffectDetails(new() {
                    Modifiers = new[] { new EffectAttributeModifier()
                    {
                        Attribute = _health,
                        OperationType = EAttributeModifierOperationType.Add,
                        Value = -10
                    }}
                }).Build();

            var geDef = new GameplayEffectDefBuilder().
                WithName("PeriodicEffect").
                WithPolicy(new PeriodicPolicy(activeTimes, interval, tickGE))
                .Build();

            var activeSpec = _mainSystem.ApplyEffectToSelf(geDef);

            int tick = 0;
            while (!activeSpec.Expired)
            {
                yield return new WaitForSeconds(interval);
                if (activeSpec.Expired) break;
                tick++;
                _mainSystem.AttributeSystem.TryGetAttributeValue(_health, out var health);
                Assert.AreEqual(100 - 10 * tick, health.CurrentValue);
            }
        }
    }
}