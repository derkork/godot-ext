using Godot;
using JetBrains.Annotations;

namespace GodotExt
{
    [PublicAPI]
    public static class ViewportExt
    {
        
        /// <summary>
        /// Converts a 2D screen coordinate to a world coordinate by applying the inverse of the viewport's transform.
        /// </summary>
        public static Vector2 ScreenToWorld(this Viewport viewport, Vector2 screenPosition)
        {
            return screenPosition * viewport.CanvasTransform;
        }

        /// <summary>
        /// Converts a world coordinate to a 2D screen coordinate by applying the viewport's transform.
        /// </summary>
        public static Vector2 WorldToScreen(this Viewport viewport, Vector2 worldPosition)
        {
            return viewport.CanvasTransform * worldPosition;
        }
    }
}