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

    public interface IAbility 
    {
        string Name { get; }
        AbilityTags Tags { get; }
        TContext GetContext<TContext>() where TContext : IAbilityContext;
        IAbilityCondition[] Conditions { get; }
    }

    /// <summary>
    /// Override this to create new ability SO with a new abstract ability
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbilitySO<T> : ScriptableObject, IAbility, IAbilityCreator<T> where T : AbilitySpec, new()
    {
        public string Name => name;

        [field: SerializeField] public AbilityTags Tags { get; private set; } = new();

        [Tooltip("Context of ability such as paramaters, effect, etc. Only one of each type is allowed.")]
        [field: SerializeReference, SubclassSelector] public IAbilityContext[] Contexts
        { get; private set; } = Array.Empty<IAbilityContext>();

        [Tooltip("Custom condition to active ability.")]
        [field: SerializeReference, SubclassSelector] public IAbilityCondition[] Conditions
        { get; private set; } = Array.Empty<IAbilityCondition>();

        public TContext GetContext<TContext>() where TContext : IAbilityContext
        {
            return Contexts.OfType<TContext>().FirstOrDefault();
        }

        T IAbilityCreator<T>.CreateAbilitySpec(AbilitySystemBehaviour owner)
        {
            var ability = InternalCreateAbility();
            ability.InitAbility(owner, this);
            return ability;
        }

        protected virtual T InternalCreateAbility() => new();
    }
}