using System.Collections.Generic;
using System.Linq;
using H2V.GameplayAbilitySystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    public interface IEffectContext
    {
    }

    /// <summary>
    /// Data structure that stores an instigator and related data, such as positions and targets
    /// Games can subclass this structure and add game-specific information
    /// It is passed throughout effect execution so it is a great place to track transient information about an execution
    /// </summary>
    public class GameplayEffectContextHandle
    {
        private AbilitySystemComponent _instigatorAbilitySystem;
        public AbilitySystemComponent InstigatorAbilitySystem => _instigatorAbilitySystem;

        public List<IEffectContext> EffectContexts { get; private set; } = new();
        private GameObject _instigator;


        public GameplayEffectContextHandle(GameObject instigator, params IEffectContext[] contexts)
        {
            AddInstigator(instigator);
            EffectContexts.AddRange(contexts);
        }

        public GameplayEffectContextHandle(AbilitySystemComponent asc, params IEffectContext[] contexts)
        {
            AddInstigator(asc);
            EffectContexts.AddRange(contexts);
        }

        public virtual bool IsValid() => true;

        public void AddInstigator(GameObject instigator)
        {
            _instigator = instigator;
            _instigatorAbilitySystem = instigator.GetComponent<AbilitySystemComponent>();
        }

        public void AddInstigator(AbilitySystemComponent asc)
        {
            _instigator = asc.gameObject;
            _instigatorAbilitySystem = asc;
        }

        public void AddContext(IEffectContext context)
        {
            EffectContexts.Add(context);
        }

        public T GetContext<T>() where T : IEffectContext
            => EffectContexts.OfType<T>().FirstOrDefault();
    }
}