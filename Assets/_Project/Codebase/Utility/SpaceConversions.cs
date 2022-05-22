using UnityEngine;

namespace VoxelSim.Utility
{
    public static class SpaceConversions
    {
        public static Vector2Int WorldToRegion(in Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / Region.WorldSpaceSize.x), 
                Mathf.FloorToInt(worldPos.y / Region.WorldSpaceSize.y));
        }

        public static Vector2Int WorldToChunk(in Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / Chunk.WorldSpaceSize.x), 
                Mathf.FloorToInt(worldPos.y / Chunk.WorldSpaceSize.y));
        }

        public static Vector2Int WorldToPixel(in Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / Pixel.WorldSpaceSize.x),
                Mathf.FloorToInt(worldPos.y / Pixel.WorldSpaceSize.y));
        }

        public static Vector2 RegionToWorld(in Vector2Int regionPos)
        {
            return new Vector2(
                regionPos.x * Region.WorldSpaceSize.x,
                regionPos.y * Region.WorldSpaceSize.y);
        }

        public static Vector2Int RegionToChunk(in Vector2Int regionPos)
        {
            return regionPos * Region.SIZE;
        }

        public static Vector2 PixelToWorld(in Vector2Int pixelPos, in Chunk chunk)
        {
            return pixelPos * Pixel.WorldSpaceSize + chunk.WorldSpaceOrigin;
        }

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