using UnityEngine;

namespace VoxelSim.Utility
{
    public static class IndexConversions
    {
        public static int Index2DTo1D(in Vector2Int index, in int size)
        {
            return index.x * size + index.y;
        }

        public static Vector2Int Index1DTo2D(in int index, in int size)
        {
            int xIndex = index / size;
            int yIndex = index % size;
            
            return new Vector2Int(xIndex, yIndex);
        }

        public static int WorldToChunkIndex(in Vector2 worldPos, in Region region)
        {
            return Index2DTo1D(SpaceConversions.WorldToChunk(worldPos) - region.ChunkSpaceOrigin, Region.SIZE);
        }

        public static int WorldToPixelIndex(in Vector2 worldPos, in Chunk chunk)
        {
            return Index2DTo1D(SpaceConversions.WorldToPixel(worldPos) - chunk.PixelSpaceOrigin, Chunk.SIZE);
        }

        public static Vector2 PixelIndexToWorld(in int index, in Chunk chunk)
        {
            Vector2Int posInChunk = Index1DTo2D(index, Chunk.SIZE);
            return SpaceConversions.PixelToWorld(posInChunk, chunk);
        }
    }
}