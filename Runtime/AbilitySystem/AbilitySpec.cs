using System.Collections.Generic;
using System.Linq;
using H2V.GameplayAbilitySystem.AbilitySystem.Components;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Helper;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AbilitySystem
{
    public class AbilitySpec
    {
        private bool _isActive;
        public bool IsActive => _isActive;

        private AbilitySO _abilityDef;
        public AbilitySO AbilityDef => _abilityDef;

        private AbilitySystemBehaviour _owner;
        public AbilitySystemBehaviour Owner => _owner;


        /// Eg. Ability of a bullet, the bullet itself is the owner but source is player or the gun
        public AbilitySystemBehaviour Source { get; set; }

        // Targets might be empty in some cases, all targets must be unique so I use HashSet
        public HashSet<AbilitySystemBehaviour> Targets { get; private set; } = new();

        /// <summary>
        /// Initiation method of ability
        /// Source is the owner when inital, but you can change source later
        /// </summary>
        /// <param name="owner">Owner of this ability</param>
        /// <param name="ability">Ability's data SO</param>
        public virtual void InitAbility(AbilitySystemBehaviour owner, AbilitySO ability)
        {
            _owner = owner;
            _abilityDef = ability;
            Source = owner;
            Targets.Clear();
        }

        public virtual void InitTargets(params AbilitySystemBehaviour[] targets)
        {
            Targets.Clear();
            Targets.UnionWith(targets);
        }

        public virtual bool TryActiveAbility()
        {
            if (_abilityDef == null)
            {
                // TODO: Implement log system that can be turn off
                Debug.LogWarning("GameplayAbilitySpec::TryActiveAbility:: Try to active a Ability with null data");
                return false;
            }

            if (_owner == null)
            {
                Debug.LogWarning($"GameplayAbilitySpec::TryActiveAbility::" +
                    $" Try to active a Ability [{_abilityDef.name}] with invalid owner");
                return false;
            }

            _owner.TryActiveAbility(this);
            return true;
        }

        /// <summary>
        /// Need the owner to active so we could use the coroutine
        /// </summary>
        /// <returns></returns>
        public virtual bool CanActiveAbility()
        {
            return !_isActive && _owner != null && _owner.isActiveAndEnabled 
                && DoesSystemsSatisfyTagRequirements() && IsPassAllCondition() && !IsBlockedByOtherAbility();
        }

        /// <summary>
        /// This ability can only active if the Owner system has all the required tags
        /// and none of the Ignore tags
        /// </summary>
        protected virtual bool DoesSystemsSatisfyTagRequirements()
        {
            return _owner.IsSatisfyTagRequirements(_abilityDef.Tags.OwnerTags) &&
                Source.IsSatisfyTagRequirements(_abilityDef.Tags.SourceTags);
        }
        
        protected virtual bool IsPassAllCondition()
        {
            if (_abilityDef.Conditions.Length <= 0) return true;
            
            foreach (var condition in _abilityDef.Conditions)
            {
                if (!condition.IsPass(this)) return false;
            }
            return true;
        }

        protected virtual bool IsBlockedByOtherAbility()
        {
            foreach (var abilitySpec in Owner.GrantedAbilities)
            {
                var blockTags = abilitySpec.AbilityDef.Tags.BlockAbilityWithTags;
                if (blockTags.Contains(_abilityDef.Tags.AbilityTag))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Not the same as granted a ability, ability might be granted but not activate yet
        /// Note: Targets must be setup before activate ability
        /// </summary>
        public void ActivateAbility()
        {
            InternalActiveAbility();
            _owner.TagSystem.AddTags(_abilityDef.Tags.ActivationTags);
        }

        private void InternalActiveAbility()
        {
            _isActive = true;
            RemoveInappropriateTargets();
            CancelTargetsAbilities();
            OnAbilityActive();
        }

        /// <summary>
        /// Will be called by <see cref="InternalActiveAbility"/> when the ability is active, 
        /// implement this for custom logic
        /// </summary>
        protected virtual void OnAbilityActive() { }

        private void CancelTargetsAbilities()
        {
            foreach (var target in Targets)
            {
                var cancelTags = _abilityDef.Tags.CancelAbilityWithTags;
                foreach (var abilitySpec in target.GrantedAbilities)
                {
                    if (cancelTags.Contains(abilitySpec.AbilityDef.Tags.AbilityTag))
                        abilitySpec.EndAbility();
                }
            } 
        }

        private void RemoveInappropriateTargets()
        {
            Targets.RemoveWhere(target => 
            {
                if (!target.IsSatisfyTagRequirements(_abilityDef.Tags.TargetTags))
                    return true;
                return false;
            });
        }

        /// <summary>
        /// Not the same as ability being removed, ability ended but still in the system
        /// This should always be called
        /// </summary>
        public void EndAbility()
        {
            if (!_isActive || _owner == null) return;

            _isActive = false;
            _owner.TagSystem.RemoveTags(_abilityDef.Tags.ActivationTags);
            Targets.Clear();
            OnAbilityEnded();
        }

        protected virtual void OnAbilityEnded() { }

        public virtual void OnAbilityGranted(AbilitySpec gameplayAbilitySpec) { }

        public virtual void OnAbilityRemoved(AbilitySpec gameplayAbilitySpec)
        {
            EndAbility();
        }
    }
}