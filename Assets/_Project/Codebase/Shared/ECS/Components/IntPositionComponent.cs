using Unity.Entities;
using Unity.Mathematics;

namespace PixelSim.Shared.ECS.Components
{
    public struct IntPositionComponent : IComponentData
    {
        public int2 value;
    }
}