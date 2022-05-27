using System.Collections.Generic;
using PixelSim.ECS.Components;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(fileName = "PhysicsSimulationSystem", menuName = "EntitySystems/PhysicsSimulation")]
    public sealed class PhysicsSimulationSystem : EntitySystem
    {
        public override Archetype RequiredArchetype => new Archetype(typeof(RigidbodyComponent));

        public override void FixedTick(in List<Entity> entities)
        {
            base.FixedTick(entities);

            Physics2D.Simulate(Time.fixedDeltaTime);
        }
    }
}