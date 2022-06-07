using Unity.Entities;

namespace PixelSim.Gameplay.ECS.BufferElements
{
    [InternalBufferCapacity(GameConstants.CHUNK_SIZE_SQR)]
    public struct ChunkPixelBufferElement : IBufferElementData
    {
        public PixelMaterialType materialType;
    }
}