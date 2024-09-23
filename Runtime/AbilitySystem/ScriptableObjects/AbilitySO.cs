using System;
using System.Linq;
using H2V.GameplayAbilitySystem.AbilitySystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects
{
    public interface IAbilityCreator<T> where T : AbilitySpec, new()
    {
        T CreateAbilitySpec(AbilitySystemBehaviour owner);
    }

    public abstract class AbilitySO : ScriptableObject, IAbilityCreator<AbilitySpec> 
    {
        [field: SerializeField] public AbilityTags Tags { get; private set; } = new();

        [field: SerializeReference, SubclassSelector,
        Tooltip("Context of ability such as paramaters, effect, etc. Only one of each type is allowed.")]
        public IAbilityContext[] Contexts { get; private set; } = Array.Empty<IAbilityContext>();

        [field: SerializeReference, SubclassSelector,
        Tooltip("Custom condition to active ability.")]
        public IAbilityCondition[] Conditions { get; private set; } = Array.Empty<IAbilityCondition>();

        public TContext GetContext<TContext>() where TContext : IAbilityContext
        {
            return Contexts.OfType<TContext>().FirstOrDefault();
        }

        public abstract AbilitySpec CreateAbilitySpec(AbilitySystemBehaviour owner);
    }

    /// <summary>
    /// Override this to create new ability SO with a new abstract ability
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbilitySO<T> : AbilitySO, IAbilityCreator<T> where T : AbilitySpec, new()
    {
        T IAbilityCreator<T>.CreateAbilitySpec(AbilitySystemBehaviour owner)
            => InternalCreateAbility(owner);

        public override AbilitySpec CreateAbilitySpec(AbilitySystemBehaviour owner)
            => InternalCreateAbility(owner);

        protected virtual T InternalCreateAbility(AbilitySystemBehaviour owner)
        {
            var ability = CreateAbility();
            ability.InitAbility(owner, this);
            return ability;
        }

        protected virtual T CreateAbility() => new();
    }
}