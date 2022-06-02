using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS
{
    public abstract class EntitySystem : ScriptableObject
    {
        public abstract Archetype IterationArchetype { get; }

        public virtual void InitializeEntity(Entity entity) { }
        public virtual void Tick(in List<Entity> entities) { }
        public virtual void FixedTick(in List<Entity> entities) { }
        public virtual void LateTick(in List<Entity> entities) { }
    }
}