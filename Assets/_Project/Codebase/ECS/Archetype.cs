using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelSim.ECS
{
    public struct Archetype : IEquatable<Archetype>
    {
        public HashSet<Type> componentTypes;

        private readonly ArchetypeEqualityComparer _equalityComparer;

        public Archetype(params Type[] componentTypes)
        {
            this.componentTypes = componentTypes.ToHashSet();
            _equalityComparer = new ArchetypeEqualityComparer();
        }

        public bool Contains(Archetype archetype)
        {
            foreach (Type componentType in archetype.componentTypes)
            {
                if (!componentTypes.Contains(componentType))
                    return false;
            }

            return true;
        }

        public bool Equals(Archetype other)
        {
            return _equalityComparer.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return obj is Archetype other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _equalityComparer.GetHashCode(this);
        }
    }

    public sealed class ArchetypeEqualityComparer : IEqualityComparer<Archetype>
    {
        public bool Equals(Archetype x, Archetype y)
        {
            return x.componentTypes.Count == y.componentTypes.Count && 
                   x.componentTypes.All(type => y.componentTypes.Contains(type));
        }

        public int GetHashCode(Archetype obj)
        {
            return obj.componentTypes.Aggregate(0, (current, type) => current ^ type.GetHashCode());
        }
    }
}