using System;

namespace H2V.GameplayAbilitySystem.AttributeSystem
{
    [Serializable]
    public struct Modifier
    {
        public float Additive;
        public float Multiplicative;
        public float Overriding;

        /// <summary>
        /// Overriding: Will use the last modifier Override value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Modifier operator +(Modifier a, Modifier b)
        {
            return new Modifier()
            {
                Additive = a.Additive + b.Additive,
                Multiplicative = a.Multiplicative + b.Multiplicative,
                Overriding = b.Overriding
            };
        }

        public override readonly string ToString()
            => $"Additive: {Additive},\nMultiplicative:\n{Multiplicative},\nOverriding: {Overriding}";
    }
}