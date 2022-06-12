using System.Collections.Generic;
using PixelSim.Backend;
using PixelSim.Gameplay.ECS.BufferElements;
using PixelSim.Gameplay.ECS.Components;
using PixelSim.Gameplay.ECS.Tags;
using PixelSim.Gameplay.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PixelSim.Gameplay.ECS.Systems
{
    [DisableAutoCreation]
    public partial class ChunkRendererSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private Dictionary<int2, ChunkRenderer> _chunkRendererDict;

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _chunkRendererDict = new Dictionary<int2, ChunkRenderer>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Entities
                .WithAll<ChunkRequiresTextureUpdateTag>()
                .ForEach((
                    ref IntPositionComponent position,
                    ref DynamicBuffer<ChunkPixelBufferElement> pixelBuffer,
                    in Entity entity) =>
                {
                    if (!_chunkRendererDict.ContainsKey(position.value))
                        CreateChunkRenderer(position.value);
                    _chunkRendererDict[position.value].UpdateRenderTextures(pixelBuffer.AsNativeArray());
                    commandBuffer.RemoveComponent(entity, typeof(ChunkRequiresTextureUpdateTag));
                }).WithoutBurst().Run();
        }

        private void CreateChunkRenderer(int2 position)
        {
            ChunkRenderer chunkRenderer =
                Object.Instantiate(AssetReferencer.Prefabs.ChunkRendererPrefab).GetComponent<ChunkRenderer>();
            _chunkRendererDict.Add(position, chunkRenderer);
        }
    }
}