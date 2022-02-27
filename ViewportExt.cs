using Godot;

namespace GodotExt
{
    static internal class ViewportExt
    {
        public static Vector2 ScreenToWorld(this Viewport viewport, Vector2 screenPosition)
        {
            return screenPosition * viewport.CanvasTransform;
        }

        public static Vector2 WorldToScreen(this Viewport viewport, Vector2 worldPosition)
        {
            return viewport.CanvasTransform * worldPosition;
        }
    }
}