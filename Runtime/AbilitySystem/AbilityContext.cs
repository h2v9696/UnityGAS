using System;

namespace H2V.GameplayAbilitySystem.AbilitySystem
{
    public interface IAbilityContext
    {
    }

    public class NullAbilityContext : IAbilityContext
    {
        public static NullAbilityContext Instance { get; } = new NullAbilityContext();
        private NullAbilityContext() { }
    }
}