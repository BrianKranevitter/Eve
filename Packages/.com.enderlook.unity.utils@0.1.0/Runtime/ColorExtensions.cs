using System;

using UnityEngine;

namespace Enderlook.Unity.Utils
{
    public static class ColorExtensions
    {
        private const string MUST_BE_BETWEEN_ZERO_AND_ONE = "Must be between 0 and 1.";

        /// <summary>
        /// Warps <paramref name="text"/> in HTML color tag with <paramref name="color"/>.<br/>
        /// Do the same as <see cref="ColorizeWith(string, Color)"/>.
        /// </summary>
        /// <param name="color">Color to tint <paramref name="text"/>.</param>
        /// <param name="text">Text to by tinted by <paramref name="color"/>.</param>
        /// <returns>Text with HTML color tag.</returns>
        public static string GetColorTag(this Color color, string text) => $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";

        /// <summary>
        /// Warps <paramref name="text"/> in HTML color tag with <paramref name="color"/>..<br/>
        /// Do the same as <see cref="GetColorTag(Color, string)"/>.
        /// </summary>
        /// <param name="text">Text to by tinted by <paramref name="color"/>.</param>
        /// <param name="color">Color to tint <paramref name="text"/>.</param>
        /// <returns>Text with HTML color tag.</returns>
        public static string ColorizeWith(this string text, Color color) => color.GetColorTag(text);

        /// <summary>
        /// Return the <paramref name="color"/> string version but colored with HTML color tag.
        /// </summary>
        /// <param name="color">Color to get string colored version.</param>
        /// <returns>String HTML colored version of <paramref name="color"/>.</returns>
        public static string ToColoredString(this Color color) => color.GetColorTag(color.ToString());

        /// <summary>
        /// Returns the hue of <paramref name="color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to get hue.</param>
        /// <returns>Hue of <paramref name="color"/>.</returns>
        public static float GetHue(this Color color)
        {
            Color.RGBToHSV(color, out float hue, out float _, out float _);
            return hue;
        }

        /// <summary>
        /// Returns the saturation of <paramref name="color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to get saturation.</param>
        /// <returns>Saturation of <paramref name="color"/>.</returns>
        public static float GetSaturation(this Color color)
        {
            Color.RGBToHSV(color, out float _, out float saturation, out float _);
            return saturation;
        }

        /// <summary>
        /// Returns the value of <paramref name="color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to get value.</param>
        /// <returns>Value of <paramref name="color"/>.</returns>
        public static float GetValue(this Color color)
        {
            Color.RGBToHSV(color, out float _, out float _, out float value);
            return value;
        }

        /// <summary>
        /// Set hue of <paramref name="color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to set hue.</param>
        /// <param name="hue">New hue.</param>
        public static void SetHue(this ref Color color, float hue)
        {
            if (hue > 1 || hue < 0) throw new ArgumentOutOfRangeException(MUST_BE_BETWEEN_ZERO_AND_ONE, nameof(hue));

            Color.RGBToHSV(color, out float _, out float saturation, out float value);
            color = Color.HSVToRGB(hue, saturation, value);
        }

        /// <summary>
        /// Set saturation of <paramref name="color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to set hue.</param>
        /// <param name="saturation">New hue.</param>
        public static void SetSaturation(this ref Color color, float saturation)
        {
            if (saturation > 1 || saturation < 0) throw new ArgumentOutOfRangeException(MUST_BE_BETWEEN_ZERO_AND_ONE, nameof(saturation));

            Color.RGBToHSV(color, out float hue, out float _, out float value);
            color = Color.HSVToRGB(hue, saturation, value);
        }

        /// <summary>
        /// Set value of <paramref name="color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to set value.</param>
        /// <param name="value">New value.</param>
        public static void SetValue(this ref Color color, float value)
        {
            if (value > 1 || value < 0) throw new ArgumentOutOfRangeException(MUST_BE_BETWEEN_ZERO_AND_ONE, nameof(value));

            Color.RGBToHSV(color, out float hue, out float saturation, out float _);
            color = Color.HSVToRGB(hue, saturation, value);
        }
    }
}