using System;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    [Serializable]
    public class DurationalPolicy : StackPolicy
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public bool IsResetOnStackChange { get; private set; }
        public DurationalPolicy() { }

        public DurationalPolicy(float duration, bool isResetOnStackChange = false, int stackPerActive = 1)
            : base(stackPerActive)
        {
            Duration = duration;
            IsResetOnStackChange = isResetOnStackChange;
        }

        public DurationalPolicy(float duration, IReduceStackStrategy reduceStackStrategy,
            bool isResetOnStackChange = false, int stackPerActive = 1)
            : base(reduceStackStrategy, stackPerActive)
        {
            Duration = duration;
            IsResetOnStackChange = isResetOnStackChange;
        }

        public override ActiveGameplayEffect CreateActiveEffect(GameplayEffectSpec inSpec) =>
            new DurationGameplayEffect(this, inSpec);
    }

    [Serializable]
    public class DurationGameplayEffect : StackGameplayEffect
    {
        private DurationalPolicy _policy;
        private float _duration = 0;

        public DurationGameplayEffect(DurationalPolicy policy, GameplayEffectSpec effect)
            : base(policy, effect)
        {
            _policy = policy;
            _duration = policy.Duration;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            _duration -= deltaTime;
            if (_duration <= 0)
            {
                Spec.IsExpired = true;
            } 
        }

        protected override void OnSpecStackChanged(int oldStackCount, int newStackCount)
        {
            base.OnSpecStackChanged(oldStackCount, newStackCount);
            if (!_policy.IsResetOnStackChange || newStackCount == 0) return;
            Spec.IsExpired = false;
            _duration = _policy.Duration;
        }   
    }
}