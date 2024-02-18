using System.Collections.Generic;
using H2V.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace H2V.GameplayAbilitySystem.TagSystem
{
    /// <summary>
    /// Manager what tags this game objects holding.
    /// </summary>
    public class TagSystemBehaviour : MonoBehaviour
    {
        public delegate void TagEvent(params TagSO[] tag);
        public event TagEvent TagAdded;
        public event TagEvent TagRemoved;

        [field: SerializeField] public List<TagSO> DefaultTags { get; private set; } = new();
        [field: SerializeField] public List<TagSO> GrantedTags { get; private set; } = new();

        private void Awake()
        {
            GrantedTags.AddRange(DefaultTags);
        }

        public void AddTags(params TagSO[] tags)
        {
            GrantedTags.AddRange(tags);
            TagAdded?.Invoke(tags);
        }

        public void RemoveTags(params TagSO[] tags)
        {
            foreach (var tag in tags)
            {
                GrantedTags.Remove(tag);
                TagRemoved?.Invoke(tag);
            }
        }

        /// <summary>
        /// Check if system contains this tag, or contains parent of this tag.
        /// Default depth is 3. Increase depth to check more parent tags.
        /// </summary>
        /// <param name="tagToCheck"></param>
        /// <returns></returns>
        public bool HasTag(TagSO tagToCheck, int depth = 3)
        {
            foreach (var tag in GrantedTags)
            {
                if (tag == tagToCheck) return true;
                if (tag.IsChildOf(tagToCheck, depth)) return true;
            }

            return false;
        }

        public bool HasAnyTag(TagSO[] tagsToCheck, int depth = 3)
        {
            foreach (var tag in tagsToCheck)
            {
                if (HasTag(tag, depth)) return true;
            }

            return false;
        }

        public bool HasAllTags(TagSO[] tagsToCheck, int depth = 3)
        {
            foreach (var tag in tagsToCheck)
            {
                if (!HasTag(tag, depth)) return false;
            }

            return true;
        }
    }
}