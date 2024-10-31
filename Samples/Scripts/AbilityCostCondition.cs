using System;
using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.EffectSystem;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.EffectSystem.Utilities;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Samples
{
    [Serializable]
    public class AbilityCostCondition : IAbilityCondition
    {
        private IGameplayEffectDef _costEffect;
        private AbilitySpec _abilitySpec;
        private AttributeWithValue _cost;

        public void Initialize(AbilitySpec abilitySpec)
        {
            _abilitySpec = abilitySpec;
            var context = abilitySpec.AbilityDef.GetContext<SampleAbilityEffectContext>();
            if (context == null) return;

            var cost = context.Cost;
            if (cost.Attribute == null) return;

            _costEffect = CreateCostEffect(cost.Attribute, cost.Value);
            _cost = cost;
        }

        public bool IsPass(AbilitySpec abilitySpec)
        {
            if (!CheckCost()) return false;

            var asc = abilitySpec.Owner.AbilitySystemComponent;
            asc.ApplyEffectToSelf(_costEffect);
            return true;
        }

        /// <summary>
        /// Predict if character has enough resource to perform this ability
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckCost()
        {
            if (_costEffect == null) 
            {
                Debug.LogWarning($"AbilityCostCondition::CheckCost:: Ability {_abilitySpec.AbilityDef.name} not Initialize yet");
                return true;
            }
            var asc = _abilitySpec.Owner.AbilitySystemComponent;
            var attributeSystem = asc.AttributeSystem;

            if (!attributeSystem.TryGetAttributeValue(_cost.Attribute, out var costValue)) return false;
            if (costValue.CurrentValue < _cost.Value) return false;

            return true;
        }
        
        private IGameplayEffectDef CreateCostEffect(AttributeSO attribute, float value)
        {
            var modifiers = new EffectAttributeModifier[1]
            {
                new()
                {
                    Attribute = attribute,
                    OperationType = EAttributeModifierOperationType.Add,
                    Value = -value
                } 
            };

            var effectDetails = new EffectDetails()
            {
                Modifiers = modifiers
            };
            
            return new GameplayEffectDefBuilder()
                .WithPolicy(new InstantPolicy())
                .WithEffectDetails(effectDetails)
                .Build();
        }
    }
}