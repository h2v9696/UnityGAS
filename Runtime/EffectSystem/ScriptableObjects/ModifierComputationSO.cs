using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects
{
    /// <summary>
    /// FGameplayEffectModifierMagnitude
    /// To prevent multiple instance of computation class, we use ScriptableObject as a "singleton"
    /// 
    /// - Scalable Float
    ///     Use Level the scale the value
    /// - Attribute Based
    ///     Take CurrentValue or BaseValue of a backing Attribute on the Source (Who created the GameplayEffectSpec)
    ///     or the Target (Who received the GameplayEffectSpec) and further modify it by a coefficient.
    /// - Custom Calculation Class
    /// - Set By Caller
    /// </summary>
    public abstract class ModifierComputationSO : ScriptableObject
    {
        /// <summary>
        /// Called when the spec is first initialised
        /// </summary>
        /// <param name="effectSpec">Gameplay Effect Spec</param>
        public abstract void Initialize(GameplayEffectSpec effectSpec);

        /// <summary>
        ///  Attempts to calculate the magnitude given the provided spec. 
        ///  May fail if necessary information (such as captured attributes) is missing from
        /// </summary>
        /// <param name="gameplayEffectSpec">Relevant spec to use to calculate the magnitude with</param>
        /// <param name="evaluatedMagnitude">out calculated value of the magnitude, will be set to 0f if its failed</param>
        /// <returns>true if the calculation was successful, false if it was not</returns>
        public abstract bool TryCalculateMagnitude(GameplayEffectSpec gameplayEffectSpec,
            ref float evaluatedMagnitude);
    }
}