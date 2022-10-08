using UnityEngine;

namespace Enderlook.Unity.Utils
{
    /// <summary>
    /// Determines how number are rounded.
    /// </summary>
    public enum RoundingMode
    {
        /// <summary>
        /// Values are show in decimal.
        /// </summary>
        None,

        /// <summary>
        /// Uses <see cref="Mathf.Round(float)"/>
        /// </summary>
        Round,

        /// <summary>
        /// Uses <see cref="Mathf.Ceil(float)"/>
        /// </summary>
        Ceil,

        /// <summary>
        /// Uses <see cref="Mathf.Floor(float)"/>.
        /// </summary>
        Floor,

        /// <summary>
        /// Cast value to <see cref="int"/>.
        /// </summary>
        Trunc,
    };
}
