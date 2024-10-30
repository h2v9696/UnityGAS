using System;
using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.EffectSystem;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Samples
{
    [Serializable]
    public class SampleAbilityContext : IAbilityContext
    {
        [field: SerializeField] public int Power { get; private set; }
        [field: SerializeField] public int Accuracy { get; private set; }
    }
}