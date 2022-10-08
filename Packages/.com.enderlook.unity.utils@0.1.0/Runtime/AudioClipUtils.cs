using System;

using UnityEngine;

namespace Enderlook.Unity.Utils
{
    /// <summary>
    /// Utility methods for <see cref="AudioClip"/>.
    /// </summary>
    public static class AudioClipUtils
    {
        /// <summary>
        /// Creates a new <see cref="AudioClip"/> from <paramref name="source"/> with new <paramref name="start"/> and <paramref name="length"/> values.
        /// </summary>
        /// <param name="source">Base <see cref="AudioClip"/> where data is gotten.</param>
        /// <param name="name">Name of new <see cref="AudioClip"/>.</param>
        /// <param name="start">Start position in seconds.</param>
        /// <param name="length">Lenght in seconds.</param>
        /// <returns>New <see cref="AudioClip"/> with specified duration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="name"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw if <paramref name="start"/> is negative, <paramref name="length"/> isn't greater than 0 or the sum of <paramref name="start"/> and <paramref name="length"/> is greater than <c><paramref name="source"/>.length</c>.</exception>
        public static AudioClip CreateSlice(this AudioClip source, string name, float start, float length)
        {
            // https://answers.unity.com/questions/993241/how-to-play-specific-part-of-the-audio.html

            if (source is null) throw new ArgumentNullException(nameof(source));
            if (name is null) throw new ArgumentNullException(nameof(name));
            else if (name.Length == 0) throw new ArgumentException("Can't be empty", nameof(name));
            if (start < 0) throw new ArgumentOutOfRangeException("Can't be negative", nameof(start));
            if (length < 0) throw new ArgumentOutOfRangeException("Must be greater than 0.", nameof(length));
            if (length + start > source.length) throw new ArgumentOutOfRangeException($"The sum of {nameof(start)} ({start}) and {nameof(length)} ({length}) can't be greater than {nameof(source)} length ({source.length}).");

            // Create audio clip
            int frequency = source.frequency;
            int samplesLength = (int)(frequency * length);
            AudioClip newClip = AudioClip.Create(name, samplesLength, 1, frequency, false);

            // Create buffer for samples
            float[] data = new float[samplesLength];
            source.GetData(data, (int)(frequency * start));

            // Transfer data to new clip
            newClip.SetData(data, 0);
            return newClip;
        }

        /// <summary>
        /// Creates a new <see cref="AudioClip"/> from <paramref name="source"/> with new <paramref name="start"/>.
        /// </summary>
        /// <param name="source">Base <see cref="AudioClip"/> where data is gotten.</param>
        /// <param name="start">Start position in seconds.</param>
        /// <param name="length">Lenght in seconds.</param>
        /// <returns>New <see cref="AudioClip"/> with specified duration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw if <paramref name="start"/> is negative, <paramref name="length"/> isn't greater than 0 or the sum of <paramref name="start"/> and <paramref name="length"/> is greater than <c><paramref name="source"/>.length</c>.</exception>
        public static AudioClip CreateSlice(this AudioClip source, float start, float length)
            => source.CreateSlice($"{source.name}_start_{ToTime(start)}_end_{ToTime(start + length)}]", start, length);

        /// <summary>
        /// Creates a new <see cref="AudioClip"/> from <paramref name="source"/> with new <paramref name="start"/>.
        /// </summary>
        /// <param name="source">Base <see cref="AudioClip"/> where data is gotten.</param>
        /// <param name="name">Name of new <see cref="AudioClip"/>.</param>
        /// <param name="start">Start position in seconds.</param>
        /// <returns>New <see cref="AudioClip"/> with specified duration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="name"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw if <paramref name="start"/> is negative.</exception>
        public static AudioClip CreateSlice(this AudioClip source, string name, float start)
            => source.CreateSlice(name, start, source.length - start);

        /// <summary>
        /// Creates a new <see cref="AudioClip"/> from <paramref name="source"/> with new <paramref name="start"/>.
        /// </summary>
        /// <param name="source">Base <see cref="AudioClip"/> where data is gotten.</param>
        /// <param name="start">Start position in seconds.</param>
        /// <returns>New <see cref="AudioClip"/> with specified duration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw if <paramref name="start"/> is negative.</exception>
        public static AudioClip CreateSlice(this AudioClip source, float start)
            => source.CreateSlice($"{source.name}_start_{ToTime(start)}", start, source.length - start);

        private static string ToTime(float source) => TimeSpan.FromSeconds(source).ToString(@"hh\:mm\:ss\:fff");
    }
}
