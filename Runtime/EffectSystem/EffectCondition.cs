namespace H2V.GameplayAbilitySystem.EffectSystem
{
    // Implement for custom conditions
    public interface IEffectCondition
    {
        bool IsPass(GameplayEffectSpec effectSpec);
    }
}