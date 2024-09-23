using H2V.ExtensionsCore.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "H2V/Gameplay Ability System/Attributes/Attribute")]
    public class AttributeSO : SerializableScriptableObject
    {
        [field: SerializeReference, SubclassSelector,
        Tooltip("This is calculator run when calculate init attribute value.")]
        public IAttributeValueCalculator InitialValueCalculator { get; private set; } 
            = new InitialAttributeValueCalculator();

        [field: SerializeReference, SubclassSelector,
        Tooltip("This is calculator run when calculate current attribute value.")]
        public IAttributeValueCalculator CurrentValueCalculator { get; private set; }
            = new CurrentAttributeValueCalculator();
    }    
}