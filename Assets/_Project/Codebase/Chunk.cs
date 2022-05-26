using PixelSim.Rendering;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim
{
    public sealed class Chunk
    {
        public const int SIZE = 64;
        public const int SQR_SIZE = SIZE * SIZE;
        
        public static Vector2 PixelSpaceSize => Vector2.one * SIZE;
        public static Vector2 WorldSpaceSize => PixelSpaceSize / WorldRenderer.PPU;

        public readonly Pixel[] pixels;
        
        public Vector2Int position;

        public Vector2Int ChunkSpaceOrigin => position;
        public Vector2 ChunkSpaceCenter => ChunkSpaceOrigin + Vector2.one / 2f;

        public Vector2Int PixelSpaceOrigin => position * SIZE;
        public Vector2 PixelSpaceCenter => PixelSpaceOrigin + PixelSpaceSize / 2f;

        public Vector2Int WorldSpaceOrigin => PixelSpaceOrigin / WorldRenderer.PPU;
        public Vector2 WorldSpaceCenter => WorldSpaceOrigin + WorldSpaceSize / 2f;

        public Chunk(in Vector2Int position)
        {
            pixels = new Pixel[SIZE * SIZE];
            Initialize(position);
        }

        public bool TryGetPixelAtIndex(in int x, in int y, out Pixel pixel)
        {
            pixel = default;
            
            if (x < 0 || x >= SIZE ||
                y < 0 || y >= SIZE)
                return false;

            pixel = pixels[IndexConversions.Index2DTo1D(x, y, SIZE)];
            return true;
        }
        
        public bool TryGetPixelAtIndex(in Vector2Int index, out Pixel pixel)
        {
            pixel = default;
            
            if (index.x < 0 || index.x >= SIZE ||
                index.y < 0 || index.y >= SIZE)
                return false;

            pixel = pixels[IndexConversions.Index2DTo1D(index, SIZE)];
            return true;
        }

        public void Initialize(in Vector2Int pos)
        {
            position = pos;
        }
    }
}