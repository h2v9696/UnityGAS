using System;

namespace H2V.GameplayAbilitySystem.EffectSystem.EffectConditions
{
    [Serializable]
    public class AlwaysTrue : IEffectCondition
    {
        public bool IsPass(GameplayEffectSpec effectSpec) => true;
    }
}