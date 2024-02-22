using UnityEngine;

namespace H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects
{   
    /// <summary>
    /// Config setup tag in editor
    /// </summary>
    public class TagSystemConfig : ScriptableObject
    {
        [SerializeField] private int _maxDepth = 10;
        public static int MaxDepth;

        public void Init()
        {
            MaxDepth = _maxDepth;
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnValidate()
        {
            Init();
        }
    }
}