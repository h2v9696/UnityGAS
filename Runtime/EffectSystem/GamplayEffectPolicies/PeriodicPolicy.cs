using System;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    [Serializable]
    public class PeriodicPolicy : StackPolicy
    {
        [field: SerializeField, Tooltip("Duration of the effect")]
        public int ActiveTimes { get; private set; }

        [field: SerializeField, Tooltip("Interval of the effect")]
        public float Interval { get; private set; }

        [field: SerializeField] public bool IsResetOnStackChange { get; private set; }

        [field: SerializeField] public IGameplayEffectDef[] Effects { get; private set; }

        public PeriodicPolicy() { }

        public PeriodicPolicy(int activeTimes, float interval, int stackPerActive = 1, bool isResetOnStackChange = false)
            : base(stackPerActive)
        {
            ActiveTimes = activeTimes;
            Interval = interval;
            IsResetOnStackChange = isResetOnStackChange;
        }

        public PeriodicPolicy(int activeTimes, float interval, IReduceStackStrategy reduceStackStrategy,
            bool isResetOnStackChange = false, int stackPerActive = 1)
            : base(reduceStackStrategy, stackPerActive)
        {
            ActiveTimes = activeTimes;
            Interval = interval;
            IsResetOnStackChange = isResetOnStackChange;
        }

        public PeriodicPolicy(int activeTimes, float interval, params IGameplayEffectDef[] effects)
            : this(activeTimes, interval)
        {
            Effects = effects;
        }

        public override ActiveGameplayEffect CreateActiveEffect(GameplayEffectSpec inSpec) =>
            new PeriodicGameplayEffect(this, inSpec);
    }

    [Serializable]
    public class PeriodicGameplayEffect : StackGameplayEffect
    {
        private PeriodicPolicy _policy;
        private int _activeTimes = 0;
        private float _interval = 0;

        public PeriodicGameplayEffect(PeriodicPolicy policy, GameplayEffectSpec inSpec) : base(policy, inSpec)
        {
            _policy = policy;
            _activeTimes = policy.ActiveTimes;
            _interval = _policy.Interval;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _interval -= deltaTime;
            if (_interval <= 0)
            {
                _interval = _policy.Interval;
                OnInterval();
            }
        }
        
        /// <summary>
        /// Do something on interval
        /// Override this method to do something other than apply effect
        /// </summary>
        protected virtual void OnInterval()
        {
            foreach (var effect in _policy.Effects)
            {
                _targetEffectSystem.Owner.ApplyEffectToSelf(effect);
            }
            _activeTimes--;
            if (_activeTimes <= 0) Spec.IsExpired = true;
        }

        protected override void OnSpecStackChanged(int oldStackCount, int newStackCount)
        {
            base.OnSpecStackChanged(oldStackCount, newStackCount);
            if (!_policy.IsResetOnStackChange || newStackCount == 0) return;
            Spec.IsExpired = false;
            _interval = _policy.Interval;
            _activeTimes = _policy.ActiveTimes;
        }   
    }
}