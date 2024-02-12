using UnityEngine;

namespace H2V.GameplayAbilitySystem.AttributeSystem.Components
{
    public class StatsInitializer : MonoBehaviour
    {
        [SerializeField] private AttributeSystemBehaviour _attributeSystem;
        [SerializeField] private bool _initOnStart = false;
        [SerializeReference, SubclassSelector] private IStatsProvider _statsProvider;

        private void OnValidate()
        {
            if (_attributeSystem != null) return;
            _attributeSystem = GetComponent<AttributeSystemBehaviour>();
        }

        private void Start()
        {
            if (_initOnStart) InitStats();
        }

        public void InitStats()
        {
            foreach (var stat in _statsProvider.Stats)
            {
                _attributeSystem.AddAttribute(stat.Attribute);
                _attributeSystem.SetAttributeBaseValue(stat.Attribute, stat.Value);
            }

            _attributeSystem.UpdateAttributeValues();
        }
    }
}