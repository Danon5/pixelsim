using Unity.Entities;

namespace PixelSim.Gameplay.ECS.BufferElements
{
    [InternalBufferCapacity(32 * 32)]
    public struct ChunkPixelBufferElement : IBufferElementData
    {
        public Pixel value;
        
        public static implicit operator Pixel(ChunkPixelBufferElement e) => e.value;
        public static implicit operator ChunkPixelBufferElement(Pixel e) => new ChunkPixelBufferElement { value = e };
    }
}