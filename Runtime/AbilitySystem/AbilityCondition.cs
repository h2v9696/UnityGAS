using System;

namespace H2V.GameplayAbilitySystem.AbilitySystem
{
    // Implement for custom conditions
    public interface IAbilityCondition
    {
        bool IsPass(AbilitySpec abilitySpec);
    }

    [Serializable]
    public class AlwaysTrue : IAbilityCondition
    {
        public bool IsPass(AbilitySpec abilitySpec) => true;
    }
}