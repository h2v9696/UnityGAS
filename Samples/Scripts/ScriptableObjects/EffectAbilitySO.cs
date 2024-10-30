using H2V.GameplayAbilitySystem.AbilitySystem;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Samples.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EffectAbility", menuName = "H2V/Gameplay Ability System/Sample/Effect Ability")]
    public class EffectAbilitySO : AbilitySO<EffectAbility>
    {
        [field: SerializeField]
        public GameplayEffectSO[] Effects { get; private set; }

        protected override EffectAbility CreateAbility()
        {
            return new EffectAbility(this);
        }
    }

    public class EffectAbility : AbilitySpec
    {
        private EffectAbilitySO _def;

        public EffectAbility() { }

        public EffectAbility(EffectAbilitySO abilitySO) : base()
        {
            _def = abilitySO;
        }

        protected override void OnAbilityActive()
        {
            var ownerAsc = Owner.GetComponent<AbilitySystemComponent>();
            foreach (var target in Targets)
            {
                var targetAsc = target.GetComponent<AbilitySystemComponent>();
                foreach (var effect in _def.Effects)
                {
                    var context = new GameplayEffectContextHandle(ownerAsc, new EffectAbilityContext(AbilityDef));
                    ownerAsc.ApplyEffectToTarget(targetAsc, effect, context);
                }
            }
        }
    }

    public class EffectAbilityContext : IEffectContext
    {
        public AbilitySO Ability { get; private set; }

        public EffectAbilityContext(AbilitySO ability)
        {
            Ability = ability; 
        }
    }
}