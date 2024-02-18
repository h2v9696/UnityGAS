using System;

namespace H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects
{
    public interface IAbilityContext
    {
        bool IsValid { get; }
    }

    [Serializable]
    public class NullAbilityContext : IAbilityContext
    {
        public static NullAbilityContext Instance { get; } = new NullAbilityContext();
        private NullAbilityContext() { }
    
        public bool IsValid => false;
    }
}