using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Components;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    public struct ModifierEvaluatedData
    {
        public AttributeSO Attribute;
        public EAttributeModifierOperationType OpType;
        public float Magnitude;

        public ModifierEvaluatedData(AttributeSO attribute, EAttributeModifierOperationType opType,
            int magnitude)
        {
            Attribute = attribute;
            OpType = opType;
            Magnitude = magnitude;
        }

        public ModifierEvaluatedData Clone()
        {
            return new ModifierEvaluatedData()
            {
                Attribute = Attribute,
                OpType = OpType,
                Magnitude = Magnitude
            };
        }
    }

    public readonly struct ModifierCallbackData
    {
        public readonly GameplayEffectSpec EffectSpec;
        public readonly ModifierEvaluatedData ModEvaluatedData;
        public readonly AbilitySystemComponent Owner;

        public ModifierCallbackData(GameplayEffectSpec spec, ModifierEvaluatedData modEvaluatedData,
            AbilitySystemComponent owner)
        {
            EffectSpec = spec;
            ModEvaluatedData = modEvaluatedData;
            Owner = owner;
        }
    }
}