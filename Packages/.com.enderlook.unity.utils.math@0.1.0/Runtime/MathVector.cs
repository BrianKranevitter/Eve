using UnityEngine;

namespace Enderlook.Unity.Utils.Math
{
    /// <summary>
    /// Helper methods for <see cref="Vector2"/> and <see cref="Vector3"/>.
    /// </summary>
    public static class MathVector
    {
        // https://codereview.stackexchange.com/questions/120933/calculating-distance-with-euclidean-manhattan-and-chebyshev-in-c

        /// <summary>
        /// Calculates the Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.
        /// </summary>
        /// <param name="v1">Vector to compare.</param>
        /// <param name="v2">Vector to compare.</param>
        /// <returns>Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.</returns>
        public static float EuclideanDistance(Vector2 v1, Vector2 v2)
            => Vector2.Distance(v1, v2);

        /// <summary>
        /// Calculates the Manhattan distance between <paramref name="v1"/> and <paramref name="v2"/>.
        /// </summary>
        /// <param name="v1">Vector to compare.</param>
        /// <param name="v2">Vector to compare.</param>
        /// <returns>Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.</returns>
        public static float ManhattanDistance(Vector2 v1, Vector2 v2)
        {
            Vector2 difference = v1 - v2;
            return Mathf.Abs(difference.x) + Mathf.Abs(difference.y);
        }

        /// <summary>
        /// Calculates the Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.
        /// </summary>
        /// <param name="v1">Vector to compare.</param>
        /// <param name="v2">Vector to compare.</param>
        /// <returns>Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.</returns>
        public static float EuclideanDistance(Vector3 v1, Vector3 v2)
            => Vector3.Distance(v1, v2);

        /// <summary>
        /// Calculates the Chebyshov distance between <paramref name="v1"/> and <paramref name="v2"/>.
        /// </summary>
        /// <param name="v1">Vector to compare.</param>
        /// <param name="v2">Vector to compare.</param>
        /// <returns>Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.</returns>
        public static float ChebyshovDistance(Vector2 v1, Vector2 v2)
        {
            Vector2 difference = v2 - v1;
            return Mathf.Max(Mathf.Abs(difference.x), Mathf.Abs(difference.y));
        }

        /// <summary>
        /// Calculates the Manhattan distance between <paramref name="v1"/> and <paramref name="v2"/>.
        /// </summary>
        /// <param name="v1">Vector to compare.</param>
        /// <param name="v2">Vector to compare.</param>
        /// <returns>Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.</returns>
        public static float ManhattanDistance(Vector3 v1, Vector3 v2)
        {
            Vector3 difference = v1 - v2;
            return Mathf.Abs(difference.x) + Mathf.Abs(difference.y) + Mathf.Abs(difference.z);
        }

        /// <summary>
        /// Calculates the Chebyshov distance between <paramref name="v1"/> and <paramref name="v2"/>.
        /// </summary>
        /// <param name="v1">Vector to compare.</param>
        /// <param name="v2">Vector to compare.</param>
        /// <returns>Euclidean distance between <paramref name="v1"/> and <paramref name="v2"/>.</returns>
        public static float ChebyshovDistance(Vector3 v1, Vector3 v2)
        {
            Vector3 difference = v2 - v1;
            return MathHelper.Max(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
        }

        /// <summary>
        /// Returns absolute <seealso cref="Vector2"/> of <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><seealso cref="Vector2"/> to become absolute.</param>
        /// <returns>Absolute <seealso cref="Vector2"/>.</returns>
        public static Vector2 Abs(this Vector2 source) => new Vector2(Mathf.Abs(source.x), Mathf.Abs(source.y));

        /// <summary>
        /// Returns absolute <seealso cref="Vector3"/> of <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><seealso cref="Vector3"/> to become absolute.</param>
        /// <returns>Absolute <seealso cref="Vector3"/>.</returns>
        public static Vector3 Abs(this Vector3 source) => new Vector3(Mathf.Abs(source.x), Mathf.Abs(source.y), Mathf.Abs(source.z));

        /// <summary>
        /// Returns absolute <seealso cref="Vector4"/> of <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><seealso cref="Vector4"/> to become absolute.</param>
        /// <returns>Absolute <seealso cref="Vector4"/>.</returns>
        public static Vector4 Abs(this Vector4 source) => new Vector4(Mathf.Abs(source.x), Mathf.Abs(source.y), Mathf.Abs(source.z), Mathf.Abs(source.w));

        /// <summary>
        /// Returns the angle of the vector in degrees. Through Tan method generated by the origin and the target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the vector start.</param>
        /// <param name="target">The point in 2D space where the vector ends.</param>
        /// <returns><seealso cref="float"/> angle in degrees.</returns>
        public static float AngleByTan(this Vector2 origin, Vector2 target)
        {
            float Atg(float tg) => Mathf.Atan(tg) * 180 / Mathf.PI;
            Vector2 tO = target - origin;
            float tan = tO.y / tO.x;
            return Mathf.Round(Atg(tan));
        }

        /// <summary>
        /// Returns the angle of the vector in degrees. Through Sin method generated by the origin and the target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the vector start.</param>
        /// <param name="target">The point in 2D space where the vector ends.</param>
        /// <returns><seealso cref="float"/> angle in degrees.</returns>
        public static float AngleBySin(this Vector2 origin, Vector2 target)
        {
            float Asin(float s) => Mathf.Asin(s) * 180 / Mathf.PI;
            Vector2 tO = target - origin;
            float magnitude = tO.magnitude;
            float sin = tO.y / magnitude;
            return Mathf.Round(Asin(sin));
        }

        /// <summary>
        /// Returns the angle of the vector in degrees. Through Cos method generated by the origin and the target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the vector start.</param>
        /// <param name="target">The point in 2D space where the vector ends.</param>
        /// <returns><seealso cref="float"/> angle in degrees.</returns>
        public static float AngleByCos(this Vector2 origin, Vector2 target)
        {
            float Acos(float c) => Mathf.Acos(c) * 180 / Mathf.PI;
            Vector2 tO = target - origin;
            float magnitude = tO.magnitude;
            float cos = tO.x / magnitude;
            return cos >= 0 ? Mathf.Round(Acos(cos)) : Mathf.Round(Acos(-cos));
        }

        /// <summary>
        /// Returns the angle of the vector in radians. Through Tan method generated by the origin and the target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the vector start.</param>
        /// <param name="target">The point in 2D space where the vector ends.</param>
        /// <returns><seealso cref="float"/> angle in radians.</returns>
        public static float AngleByTanRadian(this Vector2 origin, Vector2 target)
        {
            Vector2 tO = target - origin;
            float tan = tO.y / tO.x;
            return tan;
        }

        /// <summary>
        /// Returns the angle of the vector in radians. Through Sin method generated by the origin and the target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the vector start.</param>
        /// <param name="target">The point in 2D space where the vector ends.</param>
        /// <returns><seealso cref="float"/> angle in radians.</returns>
        public static float AngleBySinRadian(this Vector2 origin, Vector2 target)
        {
            Vector2 tO = target - origin;
            float magnitude = tO.magnitude;
            float sin = tO.y / magnitude;
            return sin;
        }

        /// <summary>
        /// Returns the angle of the vector in radians. Through Cos method generated by the origin and the target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the vector start.</param>
        /// <param name="target">The point in 2D space where the vector ends.</param>
        /// <returns><seealso cref="float"/> angle in radians.</returns>
        public static float AngleByCosRadian(this Vector2 origin, Vector2 target)
        {
            Vector2 tO = target - origin;
            float magnitude = tO.magnitude;
            float cos = tO.x / magnitude;
            return cos >= 0 ? cos : -cos;
        }

        /// <summary>
        /// Generates a projectile motion between origin and target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the projectile motion start.</param>
        /// <param name="target">The point in 2D space where the projectile motion ends.</param>
        /// <returns><seealso cref="Vector2"/> with the initial momentum.</returns>
        public static Vector2 ProjectileMotion(this Vector2 origin, Vector2 target)
        {
            float Vx(float x) => x / origin.AngleByCosRadian(target);
            float Vy(float y) => y / Mathf.Abs(origin.AngleBySinRadian(target)) + .5f * Mathf.Abs(Physics2D.gravity.y);

            float hY = target.y - origin.y;
            float dX = target.x - origin.x;

            Vector2 v0 = new Vector2(dX, 0).normalized;
            v0 *= Vx(Mathf.Abs(dX));
            v0.y = Vy(hY);

            return v0;
        }

        /// <summary>
        /// Generates a projectile motion between origin and target.
        /// </summary>
        /// <param name="origin">The point in 2D space where the projectile motion start.</param>
        /// <param name="target">The point in 2D space where the projectile motion ends.</param>
        /// <param name="t">The time of flight of a projectile motion.</param>
        /// <returns><seealso cref="Vector2"/> with the initial momentum.</returns>
        public static Vector2 ProjectileMotion(this Vector2 origin, Vector2 target, float t)
        {
            float Vx(float x) => x / origin.AngleByCosRadian(target) * t;
            float Vy(float y) => y / (Mathf.Abs(origin.AngleBySinRadian(target)) * t) + .5f * Mathf.Abs(Physics2D.gravity.y) * t;

            float hY = target.y - origin.y;
            float dX = target.x - origin.x;

            Vector2 v0 = new Vector2(dX, 0).normalized;
            v0 *= Vx(Mathf.Abs(dX));
            v0.y = Vy(hY);

            return v0;
        }

        public static Vector2Int ToVector2Int(this Vector2 source) => new Vector2Int((int)source.x, (int)source.y);

        public static Vector2Int ToVector2Int(this Vector3 source) => new Vector2Int((int)source.x, (int)source.y);

        public static Vector3Int ToVector3Int(this Vector3 source) => new Vector3Int((int)source.x, (int)source.y, (int)source.z);

        public static Vector3Int ToVector3Int(this Vector2 source) => new Vector3Int((int)source.x, (int)source.y, 0);
    }
}