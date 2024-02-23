namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    public interface IGameplayEffectPolicy
    {
        ActiveGameplayEffect CreateActiveEffect(GameplayEffectSpec spec);
    }
}