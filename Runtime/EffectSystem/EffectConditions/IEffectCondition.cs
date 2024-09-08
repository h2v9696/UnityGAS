namespace H2V.GameplayAbilitySystem.EffectSystem.EffectConditions
{
    // Implement for custom conditions
    public interface IEffectCondition
    {
        bool IsPass(GameplayEffectSpec effectSpec);
    }
}