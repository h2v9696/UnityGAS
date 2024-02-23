using H2V.GameplayAbilitySystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.EffectSystem
{
    /// <summary>
    /// Data structure that stores an instigator and related data, such as positions and targets
    /// Games can subclass this structure and add game-specific information
    /// It is passed throughout effect execution so it is a great place to track transient information about an execution
    /// </summary>
    public class GameplayEffectContext
    {
        private AbilitySystemComponent _instigatorAbilitySystem;
        public AbilitySystemComponent InstigatorAbilitySystem => _instigatorAbilitySystem;
        private GameObject _instigator;

        public bool IsValid() => true;

        public void AddInstigator(GameObject instigator)
        {
            _instigator = instigator;
            _instigatorAbilitySystem = instigator.GetComponent<AbilitySystemComponent>();
        }
    }

    /// <summary>
    /// Handle that wraps a FGameplayEffectContext or subclass, to allow it to be polymorphic and replicate properly
    ///
    /// <para>
    /// I'm not really getting this class... ported from GameplayEffectTypes.h
    /// maybe for networking and async stuff?
    /// </para>
    /// </summary>
    public class GameplayEffectContextHandle
    {
        private readonly GameplayEffectContext _data;
        public GameplayEffectContext GetContext() => _data;

        public GameplayEffectContextHandle(GameplayEffectContext data)
        {
            _data = data;
        }

        public bool IsValid() => _data != null;
    }
}