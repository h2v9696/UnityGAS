using H2V.GameplayAbilitySystem.EffectSystem;
using UnityEngine;

namespace IndiGames.GameplayAbilitySystem.EffectSystem.Components
{
    public abstract class EffectExecuteEventBase : ScriptableObject
    {
        public abstract bool PreExecute(ModifierCallbackData executeData);
        public abstract bool PostExecute(ModifierCallbackData executeData);
    }
}