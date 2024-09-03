
using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Tests.AbilitySystem
{
    [CreateAssetMenu(menuName = "H2V/Tests/Gameplay Ability System/Test Ability")]
    public class TestAbility : AbilitySO<TestAbilitySpec>
    {
        public int TestParameter = 1;
        protected override TestAbilitySpec CreateAbility()
        {
            return new TestAbilitySpec(this);
        }
    }

    public class TestAbilitySpec : AbilitySpec
    {
        public int TestParameter => _ability.TestParameter;
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
    }

    public class AlwaysFalse : IAbilityCondition
    {
        public bool IsPass(AbilitySpec abilitySpec)
        {
            return false;
        }
    }
}