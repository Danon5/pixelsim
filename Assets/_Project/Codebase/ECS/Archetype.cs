using System;

namespace PixelSim.ECS
{
    public readonly struct Archetype : IEquatable<Archetype>
    {
        public readonly Type[] componentTypes;

        public Archetype(params Type[] componentTypes)
        {
            this.componentTypes = componentTypes;
        }

        public bool Equals(Archetype other)
        {
            return Equals(componentTypes, other.componentTypes);
        }

        public override bool Equals(object obj)
        {
            return obj is Archetype other && Equals(other);
        }

        public override int GetHashCode()
        {
            return componentTypes != null ? componentTypes.GetHashCode() : 0;
        }
    }
}