using Unity.Entities;

namespace PixelSim.Gameplay.ECS.BufferElements
{
    public struct ChunkPixelBufferElement : IBufferElementData
    {
        public PixelMaterialType materialType;
    }
}