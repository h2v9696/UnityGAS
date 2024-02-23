using System;
using System.Collections;
using System.Linq;
using H2V.GameplayAbilitySystem.EffectSystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    /// <summary>
    /// Only use with effect stay in system like duration, period, or infinite effect
    /// </summary>
    [Serializable]
    public abstract class StackPolicy : IGameplayEffectPolicy
    {
        [field: SerializeField] public int StackPerActive { get; protected set; } = 1;
        
        [field: SerializeReference, SubclassSelector] 
        public IReduceStackStrategy ReduceStackStrategy { get; protected set; } 
            = new ReduceSingleStack();

        public StackPolicy()
        {
        }

        public StackPolicy(int stackPerActive)
        {
            StackPerActive = stackPerActive;
        }

        public StackPolicy(IReduceStackStrategy reduceStackStrategy, int stackPerActive = 1)
        {
            StackPerActive = stackPerActive;
            ReduceStackStrategy = reduceStackStrategy;
        }

        public virtual ActiveGameplayEffect CreateActiveEffect(GameplayEffectSpec inSpec) =>
            new StackGameplayEffect(this, inSpec);
    }

    [Serializable]
    public class StackGameplayEffect : ActiveGameplayEffect
    {
        protected IReduceStackStrategy _reduceStackStrategy;
        protected EffectSystemBehaviour _targetEffectSystem => Spec.Target.GameplayEffectSystem;

        public StackGameplayEffect(StackPolicy stackPolicy, GameplayEffectSpec effect) : base(effect)
        {
            Spec.StackCount = stackPolicy.StackPerActive;
            _reduceStackStrategy = stackPolicy.ReduceStackStrategy;
        }

        public override bool TrySelfActiveEffect()
        {
            var appliedEffects = _targetEffectSystem.AppliedEffects;
            var existStackableEffect = appliedEffects.FirstOrDefault(appliedEffect 
                => Spec.IsStackableWith(appliedEffect.Spec));
            
            if (existStackableEffect != null && existStackableEffect.IsValid())
            {
                if (existStackableEffect.StackCount == existStackableEffect.Spec.StackingDetails.StackLimitCount)
                {
                    // Do nothing if stack already reach limit and there no handle
                    if (!HandleActiveGameplayEffectStackOverflow(existStackableEffect)) return true;
                }

                var newStackCount = existStackableEffect.StackCount + StackCount;
                existStackableEffect.UpdateStackCount(newStackCount);

                return true;
            }

            // Fisrt time stack effect active then add modifier normally
            return false;
        }
        
        /// <summary>
        /// Call back when stack changes, derived for custom logic when stack changed
        /// </summary>
        /// <param name="otherSpec"></param>
        protected override void OnSpecStackChanged(int oldStackCount, int newStackCount)
        {
            base.OnSpecStackChanged(oldStackCount, newStackCount);
            if (oldStackCount == newStackCount) return;
            _targetEffectSystem.UpdateAttributeSystemModifiers();
        }

        /// <summary>
        /// Reduce stack and check if can remove from system
        /// </summary>
        /// <param name="sourceEffectSystem"></param>
        /// <returns></returns>
        public override bool CanRemoveFrom(EffectSystemBehaviour sourceEffectSystem)
        {
            _reduceStackStrategy?.ReduceStack(this);
            if (StackCount > 0) return false;
            return true;
        }

        /// <summary>
        /// Override this method to handle stack overflow, 
        /// Eg. Other effect when stack ALREADY reach limit
        /// FActiveGameplayEffectsContainer
        /// </summary>
        protected virtual bool HandleActiveGameplayEffectStackOverflow(ActiveGameplayEffect oldActiveEffect)
        {
            var stackDetails = oldActiveEffect.Spec.StackingDetails;
            var isAllowOverflowApplication = stackDetails.IsAllowOverflowApplication;

            foreach (var effect in stackDetails.OverflowEffects)
            {
                _targetEffectSystem.Owner.ApplyEffectToSelf(effect);
            }

            if (!isAllowOverflowApplication && stackDetails.IsClearStackOnOverflow)
            {
                oldActiveEffect.UpdateStackCount(0);
                _targetEffectSystem.RemoveEffect(oldActiveEffect.Spec);

            }
            return isAllowOverflowApplication;
        }
    }
}