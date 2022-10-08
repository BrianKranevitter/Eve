namespace Enderlook.Unity.Utils.Math
{
    /// <summary>
    /// Helper math methods.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Returns the highest value between the parameters.
        /// </summary>
        /// <param name="v1">Value to compare.</param>
        /// <param name="v2">Value to compare.</param>
        /// <param name="v3">Value to compare.</param>
        /// <returns>Highest value.</returns>
        public static float Max(float v1, float v2, float v3)
        {
            if (v1 > v2)
                return v1 > v3 ? v1 : v3;
            return v2 > v3 ? v2 : v3;
        }
    }
}