using UnityEngine;
using VoxelSim.Rendering;

namespace VoxelSim
{
    public sealed class Chunk
    {
        public const int SIZE = 64;

        public static Vector2 ChunkSpaceSize => Vector2.one;
        public static Vector2 WorldSpaceSize => ChunkSpaceSize * SIZE / WorldRenderer.PPU;
        
        public Vector2Int position;
        public readonly Voxel[,] voxels;

        public Vector2Int ChunkSpaceOrigin => position;
        public Vector2 ChunkSpaceCenter => ChunkSpaceOrigin + ChunkSpaceSize / 2f;

        public Vector2Int WorldSpaceOrigin => ChunkSpaceOrigin * SIZE / WorldRenderer.PPU;
        public Vector2 WorldSpaceCenter => WorldSpaceOrigin + WorldSpaceSize / 2f;

        public Chunk(in Vector2Int position)
        {
            voxels = new Voxel[SIZE, SIZE];
            Initialize(position);
        }

        public void Initialize(in Vector2Int pos)
        {
            position = pos;
            
            for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
                voxels[x, y].id = VoxelId.Dirt;
        }
    }
}