using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    public abstract class BaseEntitySystem : ScriptableObject
    {
        public abstract Archetype RequiredArchetype { get; }

        public virtual void Tick(List<Entity> entities) { }
        public virtual void FixedTick(List<Entity> entities) { }
        public virtual void LateTick(List<Entity> entities) { }
    }
}