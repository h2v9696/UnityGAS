using H2V.GameplayAbilitySystem.Components;

namespace H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects
{
    public interface IAdditionApplyEffect
    {
        void OnEffectSpecApplied(AbilitySystemComponent target);
        void OnEffectSpecRemoved(AbilitySystemComponent target);
    }
}