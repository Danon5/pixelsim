using PixelSim.Physics;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(
        fileName = nameof(PixelColliderGenerationSystem), 
        menuName = "EntitySystems/" + nameof(PixelColliderGenerationSystem))]
    public sealed class PixelColliderGenerationSystem : EntitySystem
    {
        public override Archetype IterationArchetype { get; } = new Archetype(
            typeof(PixelColliderGenerator));
        
        
    }
}