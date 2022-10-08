using System.Runtime.CompilerServices;

using UnityEngine;

namespace Enderlook.Unity.Utils
{
    /// <summary>
    /// Helper functions for <see cref="RoundingMode"/>
    /// </summary>
    public static class RoundingModeExtensions
    {
        /// <summary>
        /// Round <paramref name="value"/> with a method determined by <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Rounding mode.</param>
        /// <param name="value">Value to round.</param>
        /// <returns>Rounded value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this RoundingMode mode, float value)
        {
            switch (mode)
            {
                case RoundingMode.Round:
                    return Mathf.Round(value);
                case RoundingMode.Ceil:
                    return Mathf.Ceil(value);
                case RoundingMode.Floor:
                    return Mathf.Floor(value);
                case RoundingMode.Trunc:
                    // https://answers.unity.com/questions/626082/why-is-there-no-mathftruncate.html
                    return (int)value;
                default:
                    return value;
            }
        }

        /// <summary>
        /// Round <paramref name="value"/> with a method determined by <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Rounding mode.</param>
        /// <param name="value">Value to round.</param>
        /// <returns>Rounded value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RoundToInt(this RoundingMode mode, float value) => (int)mode.Round(value);
    }
}
