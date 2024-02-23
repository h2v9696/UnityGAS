using System.Collections.Generic;
using System.Linq;
using H2V.GameplayAbilitySystem.AttributeSystem.Components;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.EffectSystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.Components
{
    /// <summary>
    /// Wrapper around the <see cref="AttributeSystemBehaviour"/> to handle the effects
    /// for every applied effect find all of it modifiers and add it to the attribute in the <see cref="AttributeSystemBehaviour"/>
    /// </summary>
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EffectSystemBehaviour : MonoBehaviour
    {
        public delegate bool AppliedEffectDelegate(AbilitySystemComponent instigator, GameplayEffectSpec inSpec);

        public event AppliedEffectDelegate AppliedEffectToSelf;
        public event AppliedEffectDelegate AppliedEffectToTarget;

        [SerializeField] private bool _useUpdate = true;

        /// <summary>
        /// Currently there are no restrictions on add a new effect to the system except
        /// when using <see cref="ApplyEffectToSelf"/> which will check <see cref="GameplayEffectSpec.CanApply"/>
        /// </summary>
        private readonly List<ActiveGameplayEffect> _appliedEffects = new();
        public IReadOnlyList<ActiveGameplayEffect> AppliedEffects => _appliedEffects;

        public AbilitySystemComponent Owner { get; set; }

        public AttributeSystemBehaviour AttributeSystem => Owner.AttributeSystem;

        protected virtual void Awake()
        {
            Owner = GetComponent<AbilitySystemComponent>();
        }

        /// <summary>
        /// Will create a new GameplayEffectSpec from GameplayEffectSO (data)
        /// this will update the Owner of the effect to this AbilitySystem
        /// </summary>
        /// <param name="effectDef"></param>
        /// <returns></returns>
        public GameplayEffectSpec GetEffect(IGameplayEffectDef effectDef)
            => effectDef.CreateEffectSpec(Owner, Owner.MakeEffectContext());

        public void AddActiveEffect(ActiveGameplayEffect activeEffect)
        {
            _appliedEffects.Add(activeEffect);
            UpdateAttributeSystemModifiers();

            var spec = activeEffect.Spec;
            var instigator = spec.Context.GetContext().InstigatorAbilitySystem;
            AppliedEffectToSelf?.Invoke(instigator, spec);

            if (instigator == null) return;
            instigator.GameplayEffectSystem.AppliedEffectToTarget?.Invoke(Owner, spec);
        }

        public void ExpireEffectWithTagImmediately(TagSO tag)
        {
            ExpireEffectWithTag(tag);
            UpdateAttributeModifiersUsingAppliedEffects();
        }

        public void ExpireEffectWithTag(TagSO tag)
        {
            foreach (var appliedEffect in _appliedEffects)
            {
                if (appliedEffect.Expired) continue;
                if (!appliedEffect.EffectTag == tag) continue;
                appliedEffect.IsActive = false;
            }
        }

        /// <summary>
        /// Remove the effect from the system
        /// We should also remove the effect's modifiers from the attribute
        /// </summary>
        public virtual void RemoveEffect(GameplayEffectSpec effectSpec)
        {
            if (effectSpec == null || !effectSpec.IsValid())
            {
                Debug.LogWarning("Try remove invalid effect");
                return;
            }

            for (int i = _appliedEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedEffects[i];
                if (!effect.IsValid() || effectSpec.EffectDef != effect.Spec.EffectDef)
                    continue;
                RemoveEffectAtIndex(i);
            }

            // after remove the effect from system we need to update the attribute modifiers
            UpdateAttributeModifiersUsingAppliedEffects();
        }

        public ActiveGameplayEffect FindEffectByDef(IGameplayEffectDef effectDef)
            => _appliedEffects.FirstOrDefault(appliedEffect => appliedEffect.Spec.EffectDef == effectDef);

        private void Update()
        {
            if (_useUpdate) UpdateAttributeModifiersUsingAppliedEffects();
        }

        private void OnDestroy()
        {
            for (var i = _appliedEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedEffects[i];
                effect.OnRemovedFrom(this);
            }
        }

        public virtual void UpdateAttributeModifiersUsingAppliedEffects()
        {
            UpdateAttributeSystemModifiers();
            UpdateEffects();
            RemoveExpiredEffects();
            AttributeSystem.UpdateAttributeValues();
        }

        /// <summary>
        /// 1. Remove all modifier from attribute value
        /// 2. Add all modifiers from all active effects
        /// </summary>
        public virtual void UpdateAttributeSystemModifiers()
        {
            AttributeSystem.ResetAttributeModifiers();
            foreach (var effect in _appliedEffects.Where(effect => !effect.Expired))
            {
                effect.Spec.CalculateModifierMagnitudes();
                effect.ExecuteActiveEffect();
            }
        }

        private void UpdateEffects()
        {
            for (var index = 0; index < _appliedEffects.Count; index++)
            {
                var activeEffect = _appliedEffects[index];
                if (activeEffect != null && !activeEffect.Expired)
                    activeEffect.Update(Time.deltaTime);
            }
        }

        public void RemoveExpiredEffects()
        {
            for (var i = _appliedEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedEffects[i];
                if (effect != null && effect.IsValid() && !effect.Expired) continue;
                RemoveEffectAtIndex(i);
            }
        }

        public void ClearEffects()
        {
            _appliedEffects.Clear();
            UpdateAttributeSystemModifiers();
        }

        private void RemoveEffectAtIndex(int index)
        {
            var effect = _appliedEffects[index];
            if (!effect.CanRemoveFrom(this)) return;

            _appliedEffects.RemoveAt(index);
            if (effect?.Spec == null) return;
            
            effect.OnRemovedFrom(this);
        }

        [SerializeField] private List<EffectExecuteEventBase> _effectExecuteEvents = new();

        /// <summary>
        /// Called just before modifying the value of an attribute. AttributeSet can make additional modifications here. Return true to continue, or false to throw out the modification.
        /// Note this is only called during an 'execute'. E.g., a modification to the 'base value' of an attribute. It is not called during an application of a GameplayEffect, such as a 5 ssecond +10 movement speed buff.
        /// </summary>
        public bool PreGameplayEffectExecute(ModifierCallbackData executeData)
        {
            var ignore = true;
            foreach (var executeEvent in _effectExecuteEvents)
            {
                if (executeEvent == null) continue;
                ignore |= executeEvent.PreExecute(executeData);
            }

            return ignore;
        }

        public void PostGameplayEffectExecute(ModifierCallbackData executeData)
        {
            foreach (var executeEvent in _effectExecuteEvents)
            {
                if (executeEvent == null) continue;
                executeEvent.PreExecute(executeData);
            }
        }
    }
}