using UnityEngine;
using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;

namespace H2V.GameplayAbilitySystem.Components
{
    public class DefaultAbilities : MonoBehaviour, IOwnerComponent
    {
        [SerializeField]
        private AbilitySO[] _defaultAbilities;

        public void Init(AbilitySystemComponent asc)
        {
            foreach (var ability in _defaultAbilities)
            {
                asc.AbilitySystem.GiveAbility(ability);
            }
        }
    }
}