using UnityEngine;

namespace Enderlook.Unity.Utils
{
    /// <summary>
    /// Extension methods for <see cref="Collision2D"/>.
    /// </summary>
    public static class Collision2DExtensions
    {
        /// <summary>
        /// Calculates the collision impulse of a <see cref="Collision2D"/>.
        /// </summary>
        /// <param name="source"><see cref="Collision2D"/> used to calculate the impulse.</param>
        /// <returns>Collision impulse of <paramref name="source"/>.</returns>
        public static float GetCollisionImpulse(this Collision2D source)
        {
            float impulse = 0;
            for (int i = 0; i < source.contacts.Length; i++)
                impulse += source.contacts[i].normalImpulse;
            return impulse;
        }
    }
}
