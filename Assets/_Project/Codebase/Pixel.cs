using PixelSim.Rendering;
using UnityEngine;

namespace PixelSim
{
    public struct Pixel
    {
        public static Vector2 PixelSpaceSize => Vector2.one;
        public static Vector2 WorldSpaceSize => PixelSpaceSize / WorldRenderer.PPU; 
        
        public PixelId id;

        public Pixel(PixelId id)
        {
            this.id = id;
        }
    }
}