using System;

namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    public interface IReduceStackStrategy
    {
        void ReduceStack(ActiveGameplayEffect activeEffect);
    }

    [Serializable]
    public class DoNothing : IReduceStackStrategy
    {
        public void ReduceStack(ActiveGameplayEffect activeEffect)
        {
            // Do nothing
        }
    }
    
    [Serializable]
    public class ReduceSingleStack : IReduceStackStrategy
    {
        public void ReduceStack(ActiveGameplayEffect activeEffect)
        {
            activeEffect.UpdateStackCount(activeEffect.StackCount - 1);
        }
    }

    [Serializable]
    public class ReduceAllStacks : IReduceStackStrategy
    {
        public void ReduceStack(ActiveGameplayEffect activeEffect)
        {
            activeEffect.UpdateStackCount(0);
        }
    }
}