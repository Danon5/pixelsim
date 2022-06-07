using UnityEngine;

namespace PixelSim.Gameplay
{
    public struct Pixel
    {
        public static Vector2 WorldSpaceSize => Vector2.one / GameConstants.PPU; 
        
        public PixelId id;

        public Pixel(PixelId id)
        {
            this.id = id;
        }
    }
}