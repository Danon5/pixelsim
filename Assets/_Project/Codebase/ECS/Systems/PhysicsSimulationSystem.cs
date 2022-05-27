using System.Collections.Generic;
using PixelSim.ECS.Components;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(
        fileName = nameof(PhysicsSimulationSystem), 
        menuName = "EntitySystems/" + nameof(PhysicsSimulationSystem))]
    public sealed class PhysicsSimulationSystem : EntitySystem
    {
        public override Archetype RequiredArchetype { get; } = new Archetype(
            typeof(RigidbodyComponent));

        public override void FixedTick(in List<Entity> entities)
        {
            base.FixedTick(entities);

            Physics2D.Simulate(Time.fixedDeltaTime);
        }
    }
}