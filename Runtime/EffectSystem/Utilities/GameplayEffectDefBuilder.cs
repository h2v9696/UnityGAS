using System.Collections.Generic;
using H2V.GameplayAbilitySystem.Components;
using H2V.GameplayAbilitySystem.EffectSystem.AdditionApplyEffects;
using H2V.GameplayAbilitySystem.EffectSystem.EffectConditions;
using H2V.GameplayAbilitySystem.EffectSystem.GamplayEffectPolicies;
using H2V.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;

namespace H2V.GameplayAbilitySystem.EffectSystem.Utilities
{
    /// <summary>
    /// For create game effect def without scriptable object
    /// </summary>
    public class GameplayEffectDefBuilder
    {
        private string _name;
        private TagSO _effectTag;
        private IGameplayEffectPolicy _policy = new InstantPolicy();
        private EffectDetails _effectDetails = new();
        private StackingDetails _stackingDetails = new();
        private List<IAdditionApplyEffect> _additionApplyEffects = new();
        private List<EffectExecutionSO> _customExecutions = new();
        private List<IEffectCondition> _applicationConditions = new();

        public GameplayEffectDefBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public GameplayEffectDefBuilder WithEffectTag(TagSO effectTag)
        {
            _effectTag = effectTag;
            return this;
        }

        public GameplayEffectDefBuilder WithPolicy(IGameplayEffectPolicy policy)
        {
            _policy = policy;
            return this;
        }

        public GameplayEffectDefBuilder WithEffectDetails(EffectDetails effectDetails)
        {
            _effectDetails = effectDetails;
            return this;
        }

        public GameplayEffectDefBuilder WithStackingDetails(StackingDetails stackingDetails)
        {
            _stackingDetails = stackingDetails;
            return this;
        }

        public GameplayEffectDefBuilder WithAdditionApplyEffects(
            params IAdditionApplyEffect[] additionApplyEffects)
        {
            _additionApplyEffects = new List<IAdditionApplyEffect>(additionApplyEffects);
            return this;
        }

        public GameplayEffectDefBuilder WithCustomExecutions(params EffectExecutionSO[] customExecutions)
        {
            _customExecutions = new List<EffectExecutionSO>(customExecutions);
            return this;
        }

        public GameplayEffectDefBuilder WithApplicationConditions(
            params IEffectCondition[] applicationConditions)
        {
            _applicationConditions = new List<IEffectCondition>(applicationConditions);
            return this;
        }

        public GameplayEffectDef Build()
        {
            var effect = new GameplayEffectDef(_name, _effectTag, _policy, 
                _effectDetails, _stackingDetails);
            effect.SetAdditionApplyEffects(_additionApplyEffects.ToArray());
            effect.SetCustomExecutions(_customExecutions.ToArray());
            effect.SetApplicationConditions(_applicationConditions.ToArray());
            return effect;
        }
    }

    public class GameplayEffectDef : IGameplayEffectDef
    {
        public string Name { get; private set; }

        public TagSO EffectTag { get; private set; }

        public IGameplayEffectPolicy Policy { get; private set; }

        public EffectDetails EffectDetails { get; private set; }

        public StackingDetails StackingDetails { get; private set; }

        public IAdditionApplyEffect[] AdditionApplyEffects { get; private set; }

        public EffectExecutionSO[] CustomExecutions { get; private set; }

        public IEffectCondition[] ApplicationConditions { get; private set; }

        public GameplayEffectSpec CreateEffectSpec(AbilitySystemComponent ownerSystem, GameplayEffectContextHandle context)
        {
            var effect = new GameplayEffectSpec
            {
                Context = context
            };
            effect.InitEffect(this, ownerSystem);
            return effect;
        }

        public GameplayEffectDef(string name, TagSO effectTag, IGameplayEffectPolicy policy,
            EffectDetails effectDetails, StackingDetails stackingDetails)
        {
            Name = name;
            EffectTag = effectTag;
            Policy = policy;
            EffectDetails = effectDetails;
            StackingDetails = stackingDetails;
        }

        public void SetAdditionApplyEffects(params IAdditionApplyEffect[] additionApplyEffects)
        {
            AdditionApplyEffects = additionApplyEffects;
        }

        public void SetCustomExecutions(params EffectExecutionSO[] customExecutions)
        {
            CustomExecutions = customExecutions;
        }

        public void SetApplicationConditions(params IEffectCondition[] applicationConditions)
        {
            ApplicationConditions = applicationConditions;
        }
    }
}