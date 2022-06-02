using PixelSim.Rendering;
using UnityEngine;

namespace PixelSim
{
    public struct Pixel
    {
        public static Vector2 WorldSpaceSize => Vector2.one / GameRenderData.PPU; 
        
        public PixelId id;

        public Pixel(PixelId id)
        {
            this.id = id;
        }
    }
}