using UnityEngine;

namespace VoxelSim.Utility
{
    public static class SpaceConversions
    {
        public static Vector2Int WorldToRegion(Vector2 worldPos)
        {
            Vector2 pos = worldPos / Region.WorldSpaceSize;
            return new Vector2Int(
                Mathf.FloorToInt(pos.x), 
                Mathf.FloorToInt(pos.y));
        }

        public static Vector2 RegionToWorld(Vector2Int regionPos)
        {
            return new Vector2(
                regionPos.x * Region.WorldSpaceSize.x,
                regionPos.y * Region.WorldSpaceSize.y);
        }
    }
}