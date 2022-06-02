using PixelSim.Physics;
using UnityEngine;

namespace PixelSim.ECS.Components
{
    public sealed class PixelColliderComponent : EntityComponent
    {
        [field: SerializeField] public PolygonCollider2D UnityCollider { get; set; }
        public PixelColliderGenerator ColliderGenerator { get; set; } = new PixelColliderGenerator();
    }
}