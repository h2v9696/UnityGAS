using NUnit.Framework;
using UnityEngine;
using H2V.GameplayAbilitySystem.AbilitySystem.Components;

namespace H2V.GameplayAbilitySystem.Tests.AbilitySystem
{
    public class AbilitySystemBehaviourTests
    {
        private AbilitySystemBehaviour _abilitySystem;
        private TestAbility _testAbility;
        private TestAbility _testAbility2;


        [SetUp]
        public void SetUp()
        {
            var go = new GameObject();
            _abilitySystem = go.AddComponent<AbilitySystemBehaviour>();
            _testAbility = ScriptableObject.CreateInstance<TestAbility>();
            _testAbility2 = ScriptableObject.CreateInstance<TestAbility>();
        }

        
        [Test]
        public void GiveAbility_AbilityCorrectlyAdded_NoDuplicates()
        {

            var ability = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);

            Assert.AreEqual(_testAbility, ability.Ability);
            Assert.AreEqual(1, _abilitySystem.GrantedAbilities.Count);
            
            var ability2 = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            
            Assert.AreEqual(ability, ability2);
            Assert.AreEqual(1, _abilitySystem.GrantedAbilities.Count);
        }

        [Test]
        public void TryActiveAbility_AbilityActive()
        {
            var ability = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            _abilitySystem.TryActiveAbility(ability);

            // Assert
            Assert.AreEqual(true, ability.IsActive);
        }

        [Test]
        public void RemoveAbility_AbilityRemoved()
        {
            var ability = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);

            _abilitySystem.RemoveAbility(ability);
            Assert.AreEqual(0, _abilitySystem.GrantedAbilities.Count);
        }

        [Test]
        public void RemoveAllAbilities_AllAbilitiesRemoved()
        {
            _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility2);

            Assert.AreEqual(2, _abilitySystem.GrantedAbilities.Count);

            _abilitySystem.RemoveAllAbilities();

            Assert.AreEqual(0, _abilitySystem.GrantedAbilities.Count);
        }
    }
}