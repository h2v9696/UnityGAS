using System;

namespace H2V.GameplayAbilitySystem.EffectSystem.Utilities
{
    public static class GameplayEffectUtilities
    {
        public static float ComputeStackedModifierMagnitude(float baseComputedMagnitude, int stackCount,
            EAttributeModifierOperationType modOp)
        {
            if (stackCount < 0) stackCount = 0;

            float stackMag = baseComputedMagnitude;
            
            // Override modifiers don't care about stack count at all. 
            // All other modifier ops need to subtract out their bias value in order to handle
            // stacking correctly
            if (modOp != EAttributeModifierOperationType.Override)
            {
                stackMag *= stackCount;
            }

            return stackMag;
        }
    }
}