using System;
using System.Collections;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.EffectSystem.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace H2V.GameplayAbilitySystem.Tests.EffectSystem
{
    [TestFixture]
    public class CounterPolicyTests : EffectTestFixtureBase
    {
        public class TestCounterPolicy : CounterPolicy
        {
            public Action CounterEvent { get; set; }

            public TestCounterPolicy() : base() {}
            public TestCounterPolicy(int counter) : base(counter) {}

            public override void RegistCounterEvent(CounterGameplayEffect effect)
            {
                CounterEvent += effect.ReduceCounterEvent;
            }

            public override void RemoveCounterEvent(CounterGameplayEffect effect)
            {
                CounterEvent -= effect.ReduceCounterEvent;
            }
        }

        [UnityTest]
        public IEnumerator AfterCounter_ShouldRemoveFromSystem()
        {
            var counter = 5;
            var policy = new TestCounterPolicy(counter);
            var def = new GameplayEffectDefBuilder().
                WithPolicy(policy).
                Build();
            var spec = _mainSystem.ApplyEffectToSelf(def);
            Assert.IsTrue(spec.IsValid());
            Assert.AreEqual(1, _mainSystem.GameplayEffectSystem.AppliedEffects.Count);
            yield return ReduceCounter(counter, policy);
            Assert.AreEqual(0, _mainSystem.GameplayEffectSystem.AppliedEffects.Count);
        }

        private IEnumerator ReduceCounter(int counter, TestCounterPolicy policy)
        {
            while (counter > 0)
            {
                counter--;
                policy.CounterEvent?.Invoke();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}