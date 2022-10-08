using System;

using UnityEngine;

namespace Enderlook.Unity.Utils
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Convert a <see cref="LayerMask"/> into a layer number.<br/>
        /// This should only be used if the <paramref name="layerMask"/> has a single layer.
        /// </summary>
        /// <param name="layerMask"><see cref="LayerMask"/> to convert.</param>
        /// <returns>Layer number.</returns>
        public static int ToLayer(this LayerMask layerMask)
        {
            // https://forum.unity.com/threads/get-the-layernumber-from-a-layermask.114553/#post-3021162
            int bitMask = layerMask.value;
            int result = bitMask > 0 ? 0 : 31;
            while (bitMask > 1)
            {
                bitMask >>= 1;
                result++;
            }
            return result;
        }

        /// <summary>
        /// Convert a given layer to a layer mask.<br/>
        /// <c>1 &lt;&lt; <paramref name="layer"/></c>.
        /// </summary>
        /// <param name="layer">Layer to produce a layer mask.</param>
        /// <returns>Layer mast of layer</returns>
        public static int ToLayerMask(this int layer) => 1 << layer;

        /// <summary>
        /// Convert a given layer to a layer mask.<br/>
        /// <c>1 &lt;&lt; <paramref name="layer"/></c>.
        /// </summary>
        /// <param name="layer">Layer to produce a layer mask.</param>
        /// <returns>Layer mast of layer</returns>
        public static LayerMask ToLayerMask(this LayerMask layer) => 1 << layer;

        /// <summary>
        /// Invert a layer to match exactly the opposite.
        /// </summary>
        /// <param name="layer">Layer mask to invert.</param>
        /// <returns>Inverted layermask.</returns>
        public static int InvertLayerMask(this int layer) => ~layer;

        /// <summary>
        /// Invert a layer to match exactly the opposite.
        /// </summary>
        /// <param name="layer">Layer mask to invert.</param>
        /// <returns>Inverted layermask.</returns>
        public static LayerMask InvertLayerMask(this LayerMask layer) => ~layer;

        /// <summary>
        /// Check if the <paramref name="layerToCheck"/> have <paramref name="layerTrigger"/>.
        /// </summary>
        /// <param name="layerToCheck">Layers to check if have <paramref name="layerTrigger"/>.</param>
        /// <param name="layerTrigger">Layers checked in <paramref name="layerToCheck"/>.</param>
        /// <returns>Whenever <paramref name="layerToCheck"/> has <paramref name="layerTrigger"/>.</returns>
        public static bool IsContainedIn(this LayerMask layerToCheck, LayerMask layerTrigger)
            => (1 << layerToCheck & layerTrigger) != 0;

        /// <summary>
        /// Check if the <c><paramref name="source"/>.layer</c> is one the layers from <paramref name="layerTrigger"/>.
        /// </summary>
        /// <param name="source">GameObject to check its layers.</param>
        /// <param name="layerTrigger">Layer that must be checked.</param>
        /// <returns>Whenever <c><paramref name="source"/>.layer</c> is one the layers from <paramref name="layerTrigger"/>.</returns>
        public static bool IsContainedIn(this GameObject source, LayerMask layerTrigger)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return IsContainedIn(source.layer, layerTrigger);
        }
    }
}