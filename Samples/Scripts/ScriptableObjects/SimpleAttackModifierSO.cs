
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.EffectSystem;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Samples.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SimpleAttackModifier", menuName = "H2V/Gameplay Ability System/Sample/Modifier/Simple Attack Modifier")]
    public class SimpleAttackModifier : ModifierComputationSO
    {
        [SerializeField] private AttributeSO _attackAttribute;
        [SerializeField] private AttributeSO _defendAttribute;
        [SerializeField] private AttributeSO _evasionAttribute;
         
        public override void Initialize(GameplayEffectSpec effectSpec)
        {
        }

        public override bool TryCalculateMagnitude(GameplayEffectSpec gameplayEffectSpec, ref float evaluatedMagnitude)
        {
            var context = gameplayEffectSpec.ContextHandle.GetContext<EffectAbilityContext>();
            if (context == null) return false;
            var abilityContext = context.Ability.GetContext<SampleAbilityContext>();
            if (abilityContext == null) return false;

            var sourceAttributeSystem = gameplayEffectSpec.Source.AttributeSystem;
            var targetAttributeSystem = gameplayEffectSpec.Target.AttributeSystem;
            sourceAttributeSystem.TryGetAttributeValue(_attackAttribute, out var ownerAttack);
            targetAttributeSystem.TryGetAttributeValue(_defendAttribute, out var targetDefend);
            evaluatedMagnitude = -abilityContext.Power * (targetDefend.CurrentValue / ownerAttack.CurrentValue);
            
            targetAttributeSystem.TryGetAttributeValue(_evasionAttribute, out var targetEva);
            var randomValue = abilityContext.Accuracy * targetEva.CurrentValue;
            var isMissed = Random.value > randomValue / 100f;
            if (isMissed)
            {
                Debug.Log($"{targetAttributeSystem.gameObject.name} avoided!");
                evaluatedMagnitude = 0;
            }
            return evaluatedMagnitude < 0;
        }
    }
}