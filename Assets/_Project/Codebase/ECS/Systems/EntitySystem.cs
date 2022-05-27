using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    public abstract class EntitySystem : ScriptableObject
    {
        public abstract Archetype RequiredArchetype { get; }

        public virtual void Tick(in List<Entity> entities) { }
        public virtual void FixedTick(in List<Entity> entities) { }
        public virtual void LateTick(in List<Entity> entities) { }
    }
}