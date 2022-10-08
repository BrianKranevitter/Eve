#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Enderlook.Unity.Utils
{
    public static class MouseHelper
    {
        public enum ClipPlane { Near, Far };

#if UNITY_EDITOR
        /// <summary>
        /// Get the world position coordinates of the mouse position in editor.
        /// </summary>
        /// <returns>World position coordinates.</returns>
        /// <remarks>Don't use outside Unity Editor.</remarks>
        public static Vector2 GetMouseWorldPositionInEditor() =>
            /* https://answers.unity.com/questions/1321651/i-need-to-get-a-vector2-of-the-mouse-position-whil.html
             * http://answers.unity.com/answers/1323496/view.html */
            HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint(1);
#endif

        /// <summary>
        /// Get world position coordinates of the mouse position from <see cref="Camera.main"/>.<br/>
        /// As Z-axis distance from camera will be used the Z position of the <see cref="Camera.main"/>.
        /// </summary>
        /// <returns>World position coordinates.</returns>
        public static Vector2 GetMouseWorldPositionInGame() => Camera.main.GetMouseWorldPositionInGame();

        /// <summary>
        /// Get world position coordinates of the mouse position from <see cref="Camera.main"/>.
        /// </summary>
        /// <param name="clipPlane">If <c><paramref name="clipPlane"/> == <see cref="ClipPlane.Near"/></c>, <see cref="Camera.nearClipPlane"/> will be used as Z-axis distance to <paramref name="camera"/>.<br/>
        /// If <c><paramref name="clipPlane"/> == <see cref="ClipPlane.Far"/></c>, <see cref="Camera.farClipPlane"/> will be used.</param>
        /// <returns>World position coordinates.</returns>
        public static Vector2 GetMouseWorldPositionInGame(ClipPlane clipPlane) => Camera.main.GetMouseWorldPositionInGame(clipPlane);

        /// <summary>
        /// Get world position coordinates of the mouse position from <see cref="Camera.main"/>
        /// </summary>
        /// <param name="z">Z-axis distance from camera.</param>
        /// <returns>World position coordinates.</returns>
        public static Vector2 GetMouseWorldPositionInGame(float z) => Camera.main.GetMouseWorldPositionInGame(z);

        /// <summary>
        /// Get world position coordinates of the mouse position from <paramref name="camera"/>.<br/>
        /// As Z-axis distance from camera will be used the Z position of the <paramref name="camera"/>.
        /// </summary>
        /// <param name="camera">Camera used to get world position coordinates.</param>/// <returns>World position coordinates.</returns>
        public static Vector2 GetMouseWorldPositionInGame(this Camera camera) => camera.GetMouseWorldPositionInGame(camera.transform.position.z);

        /// <summary>
        /// Get world position coordinates of the mouse position from <paramref name="camera"/>.
        /// </summary>
        /// <param name="camera">Camera used to get world position coordinates.</param>
        /// <param name="clipPlane">If <c><paramref name="clipPlane"/> == <see cref="ClipPlane.Near"/></c>, <see cref="Camera.nearClipPlane"/> will be used as Z-axis distance to <paramref name="camera"/>.<br/>
        /// If <c><paramref name="clipPlane"/> == <see cref="ClipPlane.Far"/></c>, <see cref="Camera.farClipPlane"/> will be used.</param>
        /// <returns>World position coordinates.</returns>
        public static Vector2 GetMouseWorldPositionInGame(this Camera camera, ClipPlane clipPlane) => camera.GetMouseWorldPositionInGame(clipPlane == ClipPlane.Near ? camera.nearClipPlane : camera.farClipPlane);

        /// <summary>
        /// Get world position coordinates of the mouse position from <paramref name="camera"/>.
        /// </summary>
        /// <param name="camera">Camera used to get world position coordinates.</param>
        /// <param name="z">Z-axis distance from camera.</param>
        /// <returns>World position coordinates.</returns>
        public static Vector2 GetMouseWorldPositionInGame(this Camera camera, float z) => camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));

        /// <summary>
        /// Get the viewport position coordinates of the mouse position from <see cref="Camera.main"/>.
        /// </summary>
        /// <returns>Viewport position coordinates.</returns>
        public static Vector2 GetMouseViewportPositionInGame() => Camera.main.ScreenToViewportPoint(Input.mousePosition);

        /// <summary>
        /// Get the viewport position coordinates of the mouse position from <paramref name="camera"/>.
        /// </summary>
        /// <param name="camera">Camera used to get viewport position coordinates.</param>
        /// <returns>Viewport position coordinates.</returns>
        public static Vector2 GetMouseViewportPositionInGame(this Camera camera) => camera.ScreenToViewportPoint(Input.mousePosition);
    }
}