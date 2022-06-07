using PixelSim.Gameplay.ECS.BufferElements;
using PixelSim.Gameplay.ECS.Tags;
using Unity.Entities;

namespace PixelSim.Gameplay.ECS.Systems
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
                .WithAll<ChunkRequiresInitializationTag>()
                .ForEach((
                    ref DynamicBuffer<ChunkPixelBufferElement> pixelBuffer,
                    in Entity entity) =>
                {
                    for (int i = 0; i < GameConstants.CHUNK_SIZE_SQR; i++)
                        pixelBuffer.Add(new ChunkPixelBufferElement { materialType = PixelMaterialType.Dirt });
                    commandBuffer.RemoveComponent(entity, typeof(ChunkRequiresInitializationTag));
                }).Run();
        }
    }
}  