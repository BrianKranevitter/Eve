using UnityEngine.UI;

namespace Enderlook.Unity.Utils
{
    /// <summary>
    /// Helper methods to scroll <see cref="ScrollRect"/>.
    /// </summary>
    public static class ScrollRectExtensions
    {
        /// <summary>
        /// Scrolls <paramref name="source"/> to bottom.
        /// </summary>
        /// <param name="source"><see cref="ScrollRect"/> to scroll to bottom.</param>
        public static void ScrollBottom(this ScrollRect source) => source.verticalNormalizedPosition = 0;

        /// <summary>
        /// Scrolls <paramref name="source"/> to top.
        /// </summary>
        /// <param name="source"><see cref="ScrollRect"/> to scroll to top.</param>
        public static void ScrollTop(this ScrollRect source) => source.verticalNormalizedPosition = 1;

        /// <summary>
        /// Scrolls <paramref name="source"/> to left.
        /// </summary>
        /// <param name="source"><see cref="ScrollRect"/> to scroll to left.</param>
        public static void ScrollLeft(this ScrollRect source) => source.horizontalNormalizedPosition = 0;

        /// <summary>
        /// Scrolls <paramref name="source"/> to right.
        /// </summary>
        /// <param name="source"><see cref="ScrollRect"/> to scroll to right.</param>
        public static void ScrollRight(this ScrollRect source) => source.horizontalNormalizedPosition = 1;
    }
}
