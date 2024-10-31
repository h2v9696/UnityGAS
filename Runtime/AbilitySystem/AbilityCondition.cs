using System;

namespace H2V.GameplayAbilitySystem.AbilitySystem
{
    // Implement for custom conditions
    public interface IAbilityCondition
    {
        void Initialize(AbilitySpec abilitySpec);
        bool IsPass(AbilitySpec abilitySpec);
    }

    [Serializable]
    public class AlwaysTrue : IAbilityCondition
    {
        public void Initialize(AbilitySpec abilitySpec)
        {
        }

        public bool IsPass(AbilitySpec abilitySpec) => true;
    }
}