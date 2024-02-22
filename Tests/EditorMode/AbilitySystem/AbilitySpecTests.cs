using NUnit.Framework;
using UnityEngine;
using H2V.GameplayAbilitySystem.AbilitySystem.Components;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEditor;
using H2V.ExtensionsCore.Editor.Helpers;
using H2V.GameplayAbilitySystem.AbilitySystem;

namespace H2V.GameplayAbilitySystem.Tests.AbilitySystem
{
    public class AbilitySpecTests
    {
        private AbilitySystemBehaviour _abilitySystem;
        private AbilitySystemBehaviour _otherAbilitySystem;
        private TestAbility _testAbility;
        private TestAbility _otherTestAbility;
        private TagSO _requiredTag;
        private TagSO _ignoreTag;
        private TagSO _activationTag;
        private TagSO _blockTag;
        private TagSO _cancelTag;

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject();
            _abilitySystem = go.AddComponent<AbilitySystemBehaviour>();
            var otherGo = new GameObject();
            _otherAbilitySystem = otherGo.AddComponent<AbilitySystemBehaviour>();

            _testAbility = ScriptableObject.CreateInstance<TestAbility>();
            _otherTestAbility = ScriptableObject.CreateInstance<TestAbility>();

            _requiredTag = ScriptableObject.CreateInstance<TagSO>();
            _ignoreTag = ScriptableObject.CreateInstance<TagSO>();
            _activationTag = ScriptableObject.CreateInstance<TagSO>();
            _blockTag = ScriptableObject.CreateInstance<TagSO>();
            _cancelTag = ScriptableObject.CreateInstance<TagSO>();
        }

        private void AddTagToList(ref TagSO[] listToAdd, params TagSO[] tags)
        {
            ArrayUtility.AddRange(ref listToAdd, tags);
        }

        private void RemoveTagFromList(ref TagSO[] listToAdd, params TagSO[] tags)
        {
            foreach (var tag in tags)
            {
                var index = ArrayUtility.FindIndex(listToAdd, t => t == tag);
                if (index != -1)
                ArrayUtility.RemoveAt(ref listToAdd, index);
            }
        }

        [Test]
        public void InitAbility_CorrectlyInit()
        {
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);

            Assert.AreEqual(_testAbility, abilitySpec.AbilityDef);
            Assert.AreEqual(_abilitySystem, abilitySpec.Owner);
            Assert.AreEqual(_abilitySystem, abilitySpec.Source);
            Assert.AreEqual(0, abilitySpec.Targets.Count);
        }

        [Test]
        public void CanActiveAbility_Activated_ReturnsFalse()
        {
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            _abilitySystem.TryActiveAbility(abilitySpec);
            Assert.IsFalse(abilitySpec.CanActiveAbility());
        }

        [Test]
        public void CanActiveAbility_IsNotActive_ReturnsTrue()
        {
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            Assert.IsTrue(abilitySpec.CanActiveAbility());
        }

        [Test]
        public void CanActiveAbility_InvalidOwner_ReturnsFalse()
        {
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            _abilitySystem.gameObject.SetActive(false);
            Assert.IsFalse(abilitySpec.CanActiveAbility());

            _abilitySystem.gameObject.SetActive(true);
            GameObject.DestroyImmediate(_abilitySystem.gameObject);
            Assert.IsFalse(abilitySpec.CanActiveAbility());
        }

        [Test]
        public void CanActiveAbility_DoesOwnerSatisfyTagRequirements()
        {
            // In this test Source is the same as Owner so I only test Owner
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            AddTagToList(ref _testAbility.Tags.OwnerTags.RequireTags, _requiredTag);
            Assert.IsFalse(abilitySpec.CanActiveAbility());
            _abilitySystem.TagSystem.AddTags(_requiredTag);
            Assert.IsTrue(abilitySpec.CanActiveAbility());

            AddTagToList(ref _testAbility.Tags.OwnerTags.IgnoreTags, _ignoreTag);
            _abilitySystem.TagSystem.AddTags(_ignoreTag);
            Assert.IsFalse(abilitySpec.CanActiveAbility());
            _abilitySystem.TagSystem.RemoveTags(_ignoreTag);
            Assert.IsTrue(abilitySpec.CanActiveAbility());
        }

        [Test]
        public void CanActiveAbility_IsPassAllCondition()
        {
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            _testAbility.SetPrivateArrayProperty("Conditions", new IAbilityCondition[] { new AlwaysFalse() }, true);
            Assert.IsFalse(abilitySpec.CanActiveAbility());
            _testAbility.SetPrivateArrayProperty("Conditions", new IAbilityCondition[] { new AlwaysTrue() }, true);
            Assert.IsTrue(abilitySpec.CanActiveAbility());
            _testAbility.SetPrivateArrayProperty("Conditions", 
                new IAbilityCondition[] { new AlwaysTrue(), new AlwaysFalse() }, true);
            Assert.IsFalse(abilitySpec.CanActiveAbility());
        }

        [Test]
        public void CanActiveAbility_IsBlockedByOtherAbility()
        {
            AddTagToList(ref _otherTestAbility.Tags.BlockAbilityWithTags, _blockTag);
            _testAbility.Tags.AbilityTag = _blockTag;

            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            var blockAbilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_otherTestAbility);

            _abilitySystem.TryActiveAbility(blockAbilitySpec);
            // Blocked by other ability in system
            Assert.IsFalse(_abilitySystem.TryActiveAbility(abilitySpec));
        }

        [Test]
        public void ActivateAbility_AbilityActive_SystemHasActivationTags()
        {
            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            AddTagToList(ref _testAbility.Tags.ActivationTags, _activationTag);
            _abilitySystem.TryActiveAbility(abilitySpec);
            Assert.IsTrue(abilitySpec.IsActive);

            Assert.IsTrue(_abilitySystem.TagSystem.HasTag(_activationTag));
        }

        [Test]
        public void ActivateAbility_RemoveInappropriateTargets_TargetSatisfyTagRequirements()
        {

            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            AddTagToList(ref _testAbility.Tags.TargetTags.RequireTags, _requiredTag);

            _abilitySystem.TryActiveAbility(abilitySpec, _otherAbilitySystem);
            Assert.AreEqual(0, abilitySpec.Targets.Count);

            _otherAbilitySystem.TagSystem.AddTags(_requiredTag);

            _abilitySystem.TryActiveAbility(abilitySpec, _otherAbilitySystem);

            Assert.AreEqual(1, abilitySpec.Targets.Count);
        }

        [Test]
        public void ActivateAbility_CancelTargetsAbilities()
        {
            _otherTestAbility.Tags.AbilityTag = _cancelTag;
            AddTagToList(ref _testAbility.Tags.CancelAbilityWithTags, _cancelTag);

            var otherAbilitySpec = _otherAbilitySystem.GiveAbility<TestAbilitySpec>(_otherTestAbility);
            _otherAbilitySystem.TryActiveAbility(otherAbilitySpec);

            var abilitySpec = _abilitySystem.GiveAbility<TestAbilitySpec>(_testAbility);
            _abilitySystem.TryActiveAbility(abilitySpec, _otherAbilitySystem);

            Assert.IsFalse(otherAbilitySpec.IsActive);
        }

        [Test]
        public void EndAbility_AbilityNotActive_SystemHasNoTags()
        {
            ActivateAbility_AbilityActive_SystemHasActivationTags();
            var abilitySpec = _abilitySystem.GrantedAbilities[0];
            abilitySpec.EndAbility();
            Assert.IsFalse(abilitySpec.IsActive);
            Assert.IsFalse(_abilitySystem.TagSystem.HasTag(_activationTag));
        }
    }
}