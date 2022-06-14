using PixelSim.Shared.ECS.BufferElements;
using PixelSim.Shared.ECS.Tags;
using PixelSim.Shared.Gameplay;
using Unity.Entities;

namespace PixelSim.Shared.ECS.Systems
{
    [DisableAutoCreation]
    public partial class ChunkGenerationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            
            Entities
                .WithAll<ChunkRequiresGenerationTag>()
                .ForEach((
                    ref DynamicBuffer<ChunkPixelBufferElement> pixelBuffer,
                    in Entity entity) =>
                {
                    for (int i = 0; i < GameConstants.CHUNK_SIZE_SQR; i++)
                        pixelBuffer.Add(new ChunkPixelBufferElement { materialType = PixelMaterialType.Dirt });
                    commandBuffer.RemoveComponent<ChunkRequiresGenerationTag>(entity);
                    commandBuffer.AddComponent<ChunkRequiresTextureUpdateTag>(entity);
                }).Run();
        }
    }
}  