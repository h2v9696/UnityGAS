using H2V.GameplayAbilitySystem.AbilitySystem.Components;
using H2V.GameplayAbilitySystem.AttributeSystem.Components;
using H2V.ExtensionsCore.Common;
using H2V.GameplayAbilitySystem.EffectSystem;
using H2V.GameplayAbilitySystem.EffectSystem.Components;
using UnityEngine;
using H2V.GameplayAbilitySystem.TagSystem;
using H2V.ExtensionsCore.Helper;

namespace H2V.GameplayAbilitySystem.Components
{
    /// <summary>
    /// A component to interface with 3 aspects of the AbilitySystem:
    ///
    /// <see cref="AbilitySystemBehaviour"/>
    /// <see cref="EffectSystemBehaviour"/>
    /// <see cref="AttributeSystemBehaviour"/>
    /// </summary>
    [RequireComponent(typeof(AbilitySystemBehaviour))]
    [RequireComponent(typeof(EffectSystemBehaviour))]
    [RequireComponent(typeof(AttributeSystemBehaviour))]
    public class AbilitySystemComponent : MonoBehaviour
    {
        [field: SerializeField] public TagSystemBehaviour TagSystem { get; private set; }
        [field: SerializeField] public AbilitySystemBehaviour AbilitySystem { get; private set; }
        [field: SerializeField] public AttributeSystemBehaviour AttributeSystem { get; private set; }
        [field: SerializeField] public EffectSystemBehaviour GameplayEffectSystem { get; private set; }

        private CacheableComponentGetter _componentGetter;

        private void OnValidate()
        {
            _componentGetter = this.GetOrAddComponent<CacheableComponentGetter>();
            if (!TagSystem) TagSystem = GetComponent<TagSystemBehaviour>();
            if (!AbilitySystem) AbilitySystem = GetComponent<AbilitySystemBehaviour>();
            if (!AttributeSystem) AttributeSystem = GetComponent<AttributeSystemBehaviour>();
            if (!GameplayEffectSystem) GameplayEffectSystem = GetComponent<EffectSystemBehaviour>();
        }

        public virtual void Init()
        {
            AttributeSystem.Init();
            GameplayEffectSystem.Owner = this;
        }

        public new bool TryGetComponent<T>(out T component)
            => _componentGetter.TryGetComponent(out component);

        public new T GetComponent<T>() => _componentGetter.GetComponent<T>();

        /// <summary>
        /// Create an effect spec from the effect definition, with this system as the source
        /// </summary>
        /// <param name="effectDef">The <see cref="IGameplayEffectDef"/> that are used to create the spec</param>
        /// <param name="context"></param>
        /// <returns>New effect spec based on the def</returns>
        public GameplayEffectSpec MakeOutgoingSpec(IGameplayEffectDef effectDef,
            GameplayEffectContextHandle context = null)
        {
            if (effectDef == null)
                return new GameplayEffectSpec();

            if (context == null || !context.IsValid())
                context = MakeEffectContext();

            return effectDef.CreateEffectSpec(this, context);
        }

        public GameplayEffectContextHandle MakeEffectContext()
        {
            var context = new GameplayEffectContextHandle(new GameplayEffectContext());
            context.GetContext().AddInstigator(gameObject);
            return context;
        }

        /// <summary>
        /// AbilitySystemComponent.cpp::ApplyGameplayEffectSpecToSelf::line 730
        ///
        /// Create an active effect spec, apply into the system and update the attribute accordingly in this frame
        /// </summary>
        /// <param name="inSpec"></param>
        /// <returns></returns>
        public ActiveGameplayEffect ApplyEffectToSelf(GameplayEffectSpec inSpec)
        {
            if (inSpec == null) return new ActiveGameplayEffect();

            inSpec.Target = this;

            if (!inSpec.CanApply()) return new ActiveGameplayEffect();

            inSpec.CalculateModifierMagnitudes();
            var activeEffectSpec = inSpec.CreateActiveEffectSpec();

            if (activeEffectSpec.TrySelfActiveEffect())
                return activeEffectSpec;

            activeEffectSpec.ExecuteCustomEffectOnApplied(this);
            GameplayEffectSystem.AddActiveEffect(activeEffectSpec);

            return activeEffectSpec;
        }

        public ActiveGameplayEffect ApplyEffectToSelf(IGameplayEffectDef effectDef,
            GameplayEffectContextHandle context = null)
        {
            var spec = MakeOutgoingSpec(effectDef, context);
            return ApplyEffectToSelf(spec);
        }
    }
}