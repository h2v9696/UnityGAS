using H2V.GameplayAbilitySystem.AttributeSystem;
using System.Collections.Generic;

namespace H2V.GameplayAbilitySystem.Tests.AttributeSystem
{
    public struct ModifierTestCase
    {
        public float BaseValue;
        public EffectModifier[] Modifiers;
        public float ExpectedValue;
    }

    public struct EffectModifier
    {
        public Modifier Modifier;
        public EModifierType ModifierType;
    }

    public static class ModifierTestCaseProvider
    {
        public static List<ModifierTestCase> GetTestCases()
        {
            var testCases = new List<ModifierTestCase>();

            testCases.Add(new ModifierTestCase()
            {
                BaseValue = 1,
                Modifiers = new EffectModifier[]
                {
                    new()
                    {
                        Modifier = new Modifier() { Additive = 1 },
                        ModifierType = EModifierType.External
                    }
                },
                ExpectedValue = 2
            });

            testCases.Add(new ModifierTestCase()
            {
                BaseValue = 1,
                Modifiers = new EffectModifier[]
                {
                    new()
                    {
                        Modifier = new Modifier() { Multiplicative = 1 },
                        ModifierType = EModifierType.External
                    }
                },
                ExpectedValue = 2,
            });

            testCases.Add(new ModifierTestCase()
            {
                BaseValue = 1,
                Modifiers = new EffectModifier[]
                {
                    new()
                    {
                        Modifier = new Modifier() { Overriding = 10 },
                        ModifierType = EModifierType.External
                    }
                },
                ExpectedValue = 10,
            });

            // (1 + 1) * 2 = 4
            testCases.Add(new ModifierTestCase()
            {
                BaseValue = 1,
                Modifiers = new EffectModifier[]
                {
                    new()
                    {
                        Modifier = new Modifier() { Additive = 1, Multiplicative = 1 },
                        ModifierType = EModifierType.External
                    }
                },
                ExpectedValue = 4,
            });

            testCases.Add(new ModifierTestCase()
            {
                BaseValue = 1,
                Modifiers = new EffectModifier[]
                {
                    new()
                    {
                        Modifier = new Modifier() { Additive = 1, Multiplicative = 1, Overriding = 10 },
                        ModifierType = EModifierType.External
                    }
                },
                ExpectedValue = 10,
            });

            // Prio Extern > Core
            testCases.Add(new ModifierTestCase()
            {
                BaseValue = 1,
                Modifiers = new EffectModifier[]
                {
                    new()
                    {
                        Modifier = new Modifier() { Overriding = 5 },
                        ModifierType = EModifierType.External
                    },
                    new()
                    {
                        Modifier = new Modifier() { Overriding = 10 },
                        ModifierType = EModifierType.Core
                    }
                },
                ExpectedValue = 5,
            });
            return testCases;
        }
    }
}