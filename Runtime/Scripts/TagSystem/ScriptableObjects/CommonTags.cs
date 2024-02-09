using System.Collections.Generic;
using UnityEngine;
namespace H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects
{
    /// <summary>
    /// You can easily get a tag from this scriptable object by using this method with tag name
    /// CommonTags.TryGetTag("Player", out TagSO tag);
    /// TODO: Find a better way because pass string is not good
    /// </summary>
    public class CommonTags : ScriptableObject
    {
        [field: SerializeField] public List<TagSO> Tags { get; private set; } = new();

        private static Dictionary<string, TagSO> _tagsDictionary = new();

        private void OnEnable()
        {
            _tagsDictionary.Clear();
            foreach (var tag in Tags)
            {
                _tagsDictionary.Add(tag.name, tag);
            }
        }

        private void OnValidate()
        {
            OnEnable();
        }

        public static bool TryGetTag(string tagName, out TagSO tag)
        {
            return _tagsDictionary.TryGetValue(tagName, out tag);
        }
    }
}

