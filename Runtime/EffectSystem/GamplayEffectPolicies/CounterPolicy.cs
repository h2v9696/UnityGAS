using System;
using H2V.GameplayAbilitySystem.EffectSystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    /// <summary>
    /// Can be used for counter based effect like turn-based, step counter
    /// </summary>
    [Serializable]
    public abstract class CounterPolicy : StackPolicy
    {
        [field: SerializeField] public int Counter { get; private set; }
        [field: SerializeField] public bool IsResetOnStackChange { get; private set; }

        public CounterPolicy() { }

        public CounterPolicy(int counter, int stackPerActive = 1, bool isResetOnStackChange = false)
            : base(stackPerActive)
        {
            Counter = counter;
            IsResetOnStackChange = isResetOnStackChange;
        }

        public CounterPolicy(int counter, IReduceStackStrategy reduceStackStrategy,
            bool isResetOnStackChange = false, int stackPerActive = 1)
            : base(reduceStackStrategy, stackPerActive)
        {
            Counter = counter;
            IsResetOnStackChange = isResetOnStackChange;
        }

        public abstract void RegistCounterEvent(CounterGameplayEffect effect);
        /// <summary>
        /// Event should be removed when effect expired or the spec is destroyed
        /// </summary>
        public abstract void RemoveCounterEvent(CounterGameplayEffect effect);

        public override ActiveGameplayEffect CreateActiveEffect(GameplayEffectSpec inSpec) =>
            new CounterGameplayEffect(this, inSpec);
    }

    [Serializable]
    public class CounterGameplayEffect : StackGameplayEffect
    {
        private event Action ReduceCounter;
        public void ReduceCounterEvent() => ReduceCounter?.Invoke();

        protected float _counter = 0;
        private CounterPolicy _policy;

        public CounterGameplayEffect(CounterPolicy counterPolicy, GameplayEffectSpec effect)
            : base(counterPolicy, effect)
        {
            _policy = counterPolicy;
            _counter = counterPolicy.Counter;
            _policy.RegistCounterEvent(this);
            ReduceCounter += ReduceStep;
        }

        private void ReduceStep()
        {
            _counter--;
            if (_counter <= 0)
            {
                Spec.IsExpired = true;
                Spec.Target.GameplayEffectSystem.RemoveEffect(Spec);
                RemoveRegistEvent();
            }
        }

        private void RemoveRegistEvent()
        {
            ReduceCounter -= ReduceStep;
            _policy.RemoveCounterEvent(this);
        }

        public override void OnRemovedFrom(EffectSystemBehaviour _)
        {
            RemoveRegistEvent();
        }
        
        protected override void OnSpecStackChanged(int oldStackCount, int newStackCount)
        {
            base.OnSpecStackChanged(oldStackCount, newStackCount);
            if (!_policy.IsResetOnStackChange || newStackCount == 0) return;
            Spec.IsExpired = false;
            _counter = _policy.Counter;
        }   
    }
}