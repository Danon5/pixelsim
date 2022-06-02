using UnityEngine;

namespace PixelSim.ECS.Components
{
    public sealed class PixelDataComponent : EntityComponent
    {
        [field: SerializeField] public int Width { get; set; }
        [field: SerializeField] public int Height { get; set; }
        public Pixel[] Pixels { get; set; }
    }
}