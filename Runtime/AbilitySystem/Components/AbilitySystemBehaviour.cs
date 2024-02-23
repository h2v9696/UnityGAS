using System;
using System.Collections.Generic;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.TagSystem;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AbilitySystem.Components
{
    [RequireComponent(typeof(TagSystemBehaviour))]
    public partial class AbilitySystemBehaviour : MonoBehaviour
    {
        public delegate void AbilityGranted(AbilitySpec grantedAbility);

        public event AbilityGranted AbilityGrantedEvent;

        [SerializeField] private TagSystemBehaviour _tagSystem;
        public TagSystemBehaviour TagSystem => _tagSystem;

        private List<AbilitySpec> _grantedAbilities = new();
        public IReadOnlyList<AbilitySpec> GrantedAbilities => _grantedAbilities;

        private void OnValidate()
        {
            if (!_tagSystem) _tagSystem = GetComponent<TagSystemBehaviour>();
        }

        private void OnDestroy()
        {
            RemoveAllAbilities();
        }

        /// <summary>
        /// Add/Give/Grant ability to the system. Only ability that in the system can be active
        /// There's only 1 ability per system (no duplicate ability)
        /// I used IAbilityCreator<T> to make sure that there's no mistake when creating ability
        /// Mistake Eg. GiveAbility<A>(abilitySO), but abilitySO is AbilitySO<B>
        /// </summary>
        /// <param name="ability"></param>
        /// <returns>A <see cref="AbilitySpec"/> to handle (humble object) their ability logic</returns>
        public T GiveAbility<T>(IAbilityCreator<T> ability) where T : AbilitySpec, new()
        {
            if (ability == null)
                throw new NullReferenceException("AbilitySystemBehaviour::GiveAbility:: AbilitySO is null");


            for (var index = 0; index < _grantedAbilities.Count; index++)
            {
                var abilitySpec = _grantedAbilities[index];
                // Since I asure that the type of ability is the same as the type of abilitySO
                // It is pretty safe to cast it to T
                if (abilitySpec.AbilityDef == (AbilitySO) ability) return (T) abilitySpec;
            }

            var grantedAbility = ability.CreateAbilitySpec(this);

            _grantedAbilities.Add(grantedAbility);
            OnGrantedAbility(grantedAbility);

            return grantedAbility;
        }

        public AbilitySpec GiveAbility(AbilitySO abilitySO) => GiveAbility<AbilitySpec>(abilitySO);

        private void OnGrantedAbility(AbilitySpec abilitySpec)
        {
            if (abilitySpec.AbilityDef == null) return;
            Debug.Log(
                $"AbilitySystemBehaviour::OnGrantedAbility {abilitySpec.AbilityDef.name} to {gameObject.name}");
            abilitySpec.OnAbilityGranted(abilitySpec);
            AbilityGrantedEvent?.Invoke(abilitySpec);
        }

        public bool TryActiveAbility(AbilitySpec abilitySpec, params AbilitySystemBehaviour[] targets)
        {
            if (abilitySpec.AbilityDef == null) return false;
            foreach (var ability in _grantedAbilities)
            {
                if (ability != abilitySpec) continue;
                ability.InitTargets(targets);
                if (!ability.CanActiveAbility()) continue;
                ability.ActivateAbility();
                return true;
            }
            return false;
        }

        public bool RemoveAbility(AbilitySpec abilitySpec)
        {
            var isRemoved = _grantedAbilities.Remove(abilitySpec);
            if (isRemoved)
                OnRemoveAbility(abilitySpec);
            return isRemoved;
        }

        public bool RemoveAbility(AbilitySO ability)
        {
            for (int i = _grantedAbilities.Count - 1; i >= 0; i--)
            {
                var grantedSpec = _grantedAbilities[i];
                if (grantedSpec.AbilityDef != ability) continue;
                _grantedAbilities.RemoveAt(i);
                OnRemoveAbility(grantedSpec);
                return true;
            }

            return false;
        }

        public void RemoveAllAbilities()
        {
            for (int i = _grantedAbilities.Count - 1; i >= 0; i--)
            {
                var abilitySpec = _grantedAbilities[i];
                _grantedAbilities.RemoveAt(i);
                OnRemoveAbility(abilitySpec);
            }

            _grantedAbilities = new List<AbilitySpec>();
        }

        private void OnRemoveAbility(AbilitySpec abilitySpec)
        {
            if (abilitySpec.AbilityDef == null) return;

            abilitySpec.OnAbilityRemoved(abilitySpec);
        }
    }
}