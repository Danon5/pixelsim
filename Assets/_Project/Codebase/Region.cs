using UnityEngine;
using VoxelSim.Rendering;
using VoxelSim.Utility;
using VoxelSim.WorldGeneration;

namespace VoxelSim
{
    public sealed class Region
    {
        public const int SIZE = 8;
        public const int SQR_SIZE = SIZE * SIZE;
        
        public static Vector2 RegionSpaceSize => Vector2.one;
        public static Vector2 ChunkSpaceSize => new Vector2(SIZE, SIZE);
        public static Vector2 WorldSpaceSize => ChunkSpaceSize * Chunk.SIZE / WorldRenderer.PPU;

        public Vector2Int position;
        public readonly Chunk[] chunks;

        public Vector2Int RegionSpaceOrigin => position;
        public Vector2 RegionSpaceCenter => RegionSpaceOrigin + RegionSpaceSize / 2f;

        public Vector2Int ChunkSpaceOrigin => position * SIZE;
        public Vector2 ChunkSpaceCenter => ChunkSpaceOrigin + ChunkSpaceSize / 2f;

        public Vector2Int WorldSpaceOrigin => ChunkSpaceOrigin * Chunk.SIZE / WorldRenderer.PPU;
        public Vector2 WorldSpaceCenter => WorldSpaceOrigin + WorldSpaceSize / 2f;

        public Region(in Vector2Int position)
        {
            chunks = new Chunk[SIZE * SIZE];
            Initialize(position);
        }

        public bool TryGetChunkAtIndex(in Vector2Int index, out Chunk chunk)
        {
            chunk = null;
            
            if (index.x < 0 || index.x >= SIZE ||
                index.y < 0 || index.y >= SIZE)
                return false;

            chunk = chunks[IndexConversions.Index2DTo1D(index, SIZE)];
            return true;
        }

        public void Initialize(in Vector2Int pos)
        {
            position = pos;

            Vector2Int origin = ChunkSpaceOrigin;
            Vector2Int chunkSpaceOffset = Vector2Int.zero;
            
            for (int x = 0; x < SIZE; x++)
            for (int y = 0; y < SIZE; y++)
            {
                chunkSpaceOffset.x = x;
                chunkSpaceOffset.y = y;

                int index = IndexConversions.Index2DTo1D(new Vector2Int(x, y), SIZE);

                Chunk chunk;

                if (chunks[index] == null)
                {
                    chunk = new Chunk(origin + chunkSpaceOffset);
                    chunks[index] = chunk;
                }
                else
                {
                    chunk = chunks[index];
                    chunk.Initialize(origin + chunkSpaceOffset);
                }
                
                GenerateChunkAsync(chunk);
            }
        }

        private async void GenerateChunkAsync(Chunk chunk)
        {
            await WorldGenerator.GenerateChunk(chunk);
            WorldRenderer.TryQueueChunkForRebuild(chunk);
        }
    }
}