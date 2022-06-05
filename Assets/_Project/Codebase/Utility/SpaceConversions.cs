using UnityEngine;

namespace PixelSim.Utility
{
    public static class SpaceConversions
    {
        public static Vector2 WorldToScreenGUICorrected(Vector2 worldPos, Camera camera)
        {
            return ScreenToSreenGUICorrected(camera.ScreenToWorldPoint(worldPos));
        }

        public static Vector2 ScreenToSreenGUICorrected(Vector2 screenPos)
        {
            screenPos.y = Screen.height - screenPos.y;
            return screenPos;
        }
    }
}