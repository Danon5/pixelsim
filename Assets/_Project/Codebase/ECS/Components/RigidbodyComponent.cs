using UnityEngine;

namespace PixelSim.ECS.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class RigidbodyComponent : EntityComponent
    {
        public Rigidbody2D UnityRigidbody { get; private set; }

        private void Awake()
        {
            UnityRigidbody = GetComponent<Rigidbody2D>();
        }
    }
}