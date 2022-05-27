using UnityEngine;

namespace PixelSim.ECS.Components
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class RigidbodyComponent : EntityComponent
    {
        [field: SerializeField] public Rigidbody2D UnityRigidbody { get; set; }
    }
}