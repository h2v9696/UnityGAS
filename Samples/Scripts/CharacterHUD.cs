using H2V.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using H2V.GameplayAbilitySystem.Components;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.Samples
{
    public class CharacterHUD : MonoBehaviour, IOwnerComponent
    {
        [SerializeField] private Rect _windowRect = new Rect(100, 100, 300, 300); // Window position and size
        [SerializeField] private AbilitySystemComponent _target;
        [SerializeField] private AttributeSO _hpAttribute;
        [SerializeField] private AttributeSO _atkAttribute;
        [SerializeField] private int _windowId;

        private const int BUTTONS_PER_ROWS = 2;

        private int AbilityCount => _asc?.AbilitySystem.GrantedAbilities.Count ?? 0;
        private AbilitySystemComponent _asc;

        public void Init(AbilitySystemComponent abilitySystemComponent)
        {
            _asc = abilitySystemComponent;
        }
        
        private void OnGUI()
        {
            _windowRect = GUI.Window(_windowId, _windowRect, DrawWindow, gameObject.name);
        }

        // Draw the window contents
        private void DrawWindow(int windowID)
        {
            GUILayout.Label("Select skill", GUILayout.ExpandWidth(true));
            _asc.AttributeSystem.TryGetAttributeValue(_hpAttribute, out var currentHp);
            GUILayout.Label($"HP: {currentHp.CurrentValue}", GUILayout.ExpandWidth(true));
            _asc.AttributeSystem.TryGetAttributeValue(_atkAttribute, out var currentAtk);
            GUILayout.Label($"Atk: {currentAtk.CurrentValue}", GUILayout.ExpandWidth(true));

            GUILayout.FlexibleSpace(); // Push buttons to the bottom

            int rows = Mathf.CeilToInt(AbilityCount / (float)BUTTONS_PER_ROWS);

            GUILayout.BeginVertical();

            // Begin horizontal layout for each row
            for (int row = 0; row < rows; row++)
            {
                GUILayout.BeginHorizontal();

                for (int col = 0; col < BUTTONS_PER_ROWS; col++)
                {
                    int buttonIndex = row * BUTTONS_PER_ROWS + col;
                    if (buttonIndex < AbilityCount)
                    {
                        var ability = _asc.AbilitySystem.GrantedAbilities[buttonIndex];
                        var abilityName = ability.AbilityDef.name;
                        if (GUILayout.Button(abilityName, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
                        {
                            Debug.Log($"{gameObject.name} used {abilityName}!");
                            _asc.AbilitySystem.TryActiveAbility(ability, _target.AbilitySystem);
                            ability.EndAbility();
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}