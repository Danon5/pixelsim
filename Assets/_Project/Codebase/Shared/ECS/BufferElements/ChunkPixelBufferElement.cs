using PixelSim.Shared.Gameplay;
using Unity.Entities;

namespace PixelSim.Shared.ECS.BufferElements
{
    public struct ChunkPixelBufferElement : IBufferElementData
    {
        public PixelMaterialType materialType;
    }
}