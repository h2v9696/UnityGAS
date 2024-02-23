using System;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies
{
    [Serializable]
    public class InstantPolicy : IGameplayEffectPolicy
    {
        /// <summary>
        /// It's mean instantly modify base of the attribute
        /// suitable for non-stats attribute like HP (not MaxHP)
        /// Attack can be treat as instant effect
        /// Enemy -> attack (effect) -> Player
        ///
        /// Based on GAS I would want Instant effect as a infinite effect but for now I will modify the base value
        /// </summary>
        public ActiveGameplayEffect CreateActiveEffect(GameplayEffectSpec inSpec)
            => new InstantActiveEffectPolicy(inSpec);
    }

    [Serializable]
    public class InstantActiveEffectPolicy : ActiveGameplayEffect
    {
        public InstantActiveEffectPolicy(GameplayEffectSpec inSpec) : base(inSpec) { }

        public override void ExecuteActiveEffect()
        {
            Debug.Log(
                $"DefaultEffectApplier::ApplyInstantEffect {Spec.EffectDef.Name} to system {Spec.Target.name}");
            var modifySuccess = false;
            for (var index = 0; index < Spec.EffectDef.EffectDetails.Modifiers.Length; index++)
            {
                var modifier = Spec.EffectDef.EffectDetails.Modifiers[index];
                var evalData = new ModifierEvaluatedData()
                {
                    Attribute = modifier.Attribute,
                    OpType = modifier.OperationType,
                    Magnitude = Spec.GetModifierMagnitude(index)
                };
                modifySuccess |= InternalExecuteMod(evalData);
            }

            foreach (var evalData in ComputedModifiers) modifySuccess |= InternalExecuteMod(evalData);

            // after modify the attribute this effect is now expired
            // The system only care if effect is expired or not
            IsActive = false;
        }

        public override bool TrySelfActiveEffect()
        {
            ExecuteActiveEffect();
            return true;
        }
    }
}