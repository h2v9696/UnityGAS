using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects
{   
    [CreateAssetMenu(fileName = "TagSO", menuName = "H2V/Gameplay Ability System/Tag")]
    public class TagSO : ScriptableObject
    {
        [SerializeField] private TagSO _parent;
        public TagSO Parent => _parent;

        /// <summary>
        /// <para>Check if the tag is child/descendant of other tag.</para>
        /// By default will search only 3 levels deep.
        /// </summary>
        /// <param name="otherTag">Parent/Ancestor tag to compare with</param>
        /// <param name="depthSearch">depth limit to search, increase this if needed for more complex system</param>
        /// <returns>True if this tag is a child/descendant of the other tag</returns>
        public bool IsChildOf(TagSO otherTag, int depthSearch = 3)
        {
            var currentParent = _parent;
            while (depthSearch >=  0)
            {
                if (depthSearch <= 0) return false;

                // If we have no parent, we are not a child of anything. At root
                if (currentParent == null) return false;

                // If the current tag is the same as the one we are checking, we are a child of it
                if (currentParent == otherTag) return true;

                currentParent = currentParent.Parent;
                depthSearch--;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var tagConfigPath = AssetDatabase.FindAssets("t:TagSystemConfig");
            if (tagConfigPath.Length == 0) return;
            var config = AssetDatabase.LoadAssetAtPath<TagSystemConfig>(
                AssetDatabase.GUIDToAssetPath(tagConfigPath[0]));
            if (config == null)
            {
                Debug.LogWarning($"TagConfig not found! Please create one in Tag Browser.");
                return;
            }
            config.Init();

            ValidateSelfParent();
            ValidateCircular();
            ValidateMaxDepth();
        }

        private void ValidateSelfParent()
        {
            if (_parent != this) return;
            Debug.LogError("Tag cannot be its own parent", this);
            _parent = null;
        }

        private void ValidateCircular()
        {
            if (_parent == null || !_parent.IsChildOf(this, TagSystemConfig.MaxDepth)) return;
            string errorLog = "Circular reference detected:\n";

            var child = _parent;
            while (child != this)
            {
                errorLog += child.name + " > ";
                child = child.Parent;
            }
            Debug.LogError(errorLog + name, this);
            _parent = null;
        }

        private void ValidateMaxDepth()
        {
            var depth = 0;
            var parent = _parent;
            while (parent != null)
            {
                parent = parent.Parent;
                depth++;
                if (depth > TagSystemConfig.MaxDepth)
                {
                    Debug.LogError($"Tag has too many parents. Max depth reached: {TagSystemConfig.MaxDepth}", this);
                    _parent = null;
                    break;
                }
            }
        }
#endif
    }
}