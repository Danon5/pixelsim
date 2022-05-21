using UnityEngine;
using VoxelSim.Rendering;

namespace VoxelSim
{
    public sealed class Region
    {
        public const int SIZE = 8;
        
        public static Vector2 RegionSpaceSize => Vector2.one;
        public static Vector2 ChunkSpaceSize => new Vector2(SIZE, SIZE);
        public static Vector2 WorldSpaceSize => ChunkSpaceSize * Chunk.SIZE / WorldRenderer.PPU;

        public Vector2Int position;
        public readonly Chunk[,] chunks;

        public Vector2Int RegionSpaceOrigin => position;
        public Vector2 RegionSpaceCenter => RegionSpaceOrigin + RegionSpaceSize / 2f;

        public Vector2Int ChunkSpaceOrigin => position * SIZE;
        public Vector2 ChunkSpaceCenter => ChunkSpaceOrigin + ChunkSpaceSize / 2f;

        public Vector2Int WorldSpaceOrigin => ChunkSpaceOrigin * Chunk.SIZE / WorldRenderer.PPU;
        public Vector2 WorldSpaceCenter => WorldSpaceOrigin + WorldSpaceSize / 2f;

        public Region(in Vector2Int position)
        {
            chunks = new Chunk[SIZE, SIZE];
            Initialize(position);
        }

        public void Initialize(in Vector2Int position)
        {
            this.position = position;

            Vector2Int origin = ChunkSpaceOrigin;
            Vector2Int offset = Vector2Int.zero;
            
            for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
            {
                offset.x = x;
                offset.y = y;
                
                if (chunks[x, y] == null)
                    chunks[x, y] = new Chunk(origin + offset);
                else
                    chunks[x, y].Initialize(position);
            }
        }
    }
}