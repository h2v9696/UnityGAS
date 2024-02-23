using System;
using H2V.GameplayAbilitySystem.AttributeSystem;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    [Serializable]
    public class EffectDetails
    {
        public EffectAttributeModifier[] Modifiers = Array.Empty<EffectAttributeModifier>();
        public EModifierType StackingType = EModifierType.Core;
    }
}