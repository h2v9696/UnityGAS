using System;
using UnityEngine;
using H2V.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;

namespace H2V.GameplayAbilitySystem.AttributeSystem.Components
{
    public interface IStatsProvider
    {
        AttributeWithValue[] Stats { get; }
    }

    [Serializable]
    public class InlineStatsProvider : IStatsProvider
    {
        [field: SerializeField]
        public AttributeWithValue[] Stats { get; private set; }
    }

    [Serializable]
    public class ScriptableObjectStatsProvider : IStatsProvider
    {
        [SerializeField]
        private InitializeStatsSO _initializeStats;
        public AttributeWithValue[] Stats => _initializeStats.Stats;
    }
}