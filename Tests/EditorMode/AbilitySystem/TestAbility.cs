
using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.AbilitySystem.Components;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using NUnit.Framework;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Tests.AbilitySystem
{
    [CreateAssetMenu(menuName = "H2V/Tests/Gameplay Ability System/Test Ability")]
    public class TestAbility : AbilitySO<TestAbilitySpec>
    {
        protected override TestAbilitySpec CreateAbility()
        {
            return new TestAbilitySpec(this);
        }
    }

    public class TestAbilitySpec : AbilitySpec
    {
        private TestAbility _ability;

        public TestAbilitySpec() { }

        public TestAbilitySpec(TestAbility abilitySO)
        {
            _ability = abilitySO;
        }

        protected override void OnAbilityActive()
        {
            Debug.Log($"TestAbilitySpec.OnAbilityActive()");
        }

        public override void InitAbility(AbilitySystemBehaviour owner, AbilitySO ability)
        {
            base.InitAbility(owner, ability);
            Assert.IsNotNull(_ability);
            var context = _ability.GetContext<TestContext>();
            Assert.IsNotNull(context);
            Assert.AreEqual(1, context.TestParameter);
            Debug.Log($"Ability {_ability}: context: {context} TestParameter: {context.TestParameter}");
        }

        protected override void OnAbilityEnded()
        {
            Debug.Log($"TestAbilitySpec.OnAbilityEnded()");
        }
    }

    public class AlwaysFalse : IAbilityCondition
    {
        public void Initialize(AbilitySpec abilitySpec)
        {
        }

        public bool IsPass(AbilitySpec abilitySpec)
        {
            return false;
        }
    }

    public class TestContext : IAbilityContext
    {
        public int TestParameter = 1;

        public bool IsValid => true;
    }
}