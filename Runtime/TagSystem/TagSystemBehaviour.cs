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

        protected virtual void Awake()
        {
            GrantedTags.AddRange(DefaultTags);
        }

        public virtual void AddTags(params TagSO[] tags)
        {
            GrantedTags.AddRange(tags);
            TagAdded?.Invoke(tags);
        }

        public virtual void RemoveTags(params TagSO[] tags)
        {
            foreach (var tag in tags)
            {
                GrantedTags.Remove(tag);
                TagRemoved?.Invoke(tag);
            }
        }

        public virtual bool HasTag(TagSO tagToCheck)
        {
            return GrantedTags.Contains(tagToCheck);
        }
    }
}