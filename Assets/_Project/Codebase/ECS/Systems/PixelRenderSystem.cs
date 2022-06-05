using System.Collections.Generic;
using PixelSim.Backend;
using PixelSim.ECS.Components;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(
        fileName = nameof(PixelRenderSystem), 
        menuName = "EntitySystems/" + nameof(PixelRenderSystem))]
    public sealed class PixelRenderSystem : EntitySystem
    {
        public override Archetype IterationArchetype { get; } = new Archetype(
            typeof(PixelDataComponent), typeof(PixelRendererComponent));

        public override void InitializeEntity(Entity entity)
        {
            base.InitializeEntity(entity);

            PixelDataComponent pixelData = entity.GetComponent<PixelDataComponent>();
            PixelRendererComponent pixelRenderer = entity.GetComponent<PixelRendererComponent>();

            int textureWidth = pixelData.Width;
            int textureHeight = pixelData.Height;
            int bufferSize = textureWidth * textureHeight;

            pixelRenderer.PixelBuffer = new ComputeBuffer(bufferSize, 4, ComputeBufferType.Structured);
            
            pixelRenderer.ResultTexture = new RenderTexture(textureWidth, textureHeight, 24)
            {
                enableRandomWrite = true,
                useMipMap = false,
                filterMode = FilterMode.Point
            };
            pixelRenderer.ResultTexture.Create();
            
            Vector2 worldSpaceRenderSize = new Vector2(textureWidth, textureHeight) / GameRenderData.PPU;

            pixelRenderer.SolidRenderImage.rectTransform.sizeDelta = worldSpaceRenderSize;
            pixelRenderer.SolidRenderImage.texture = pixelRenderer.ResultTexture;
        }

        public override void LateTick(in List<Entity> entities)
        {
            base.LateTick(in entities);

            foreach (Entity entity in entities)
            {
                PixelDataComponent pixelData = entity.GetComponent<PixelDataComponent>();
                PixelRendererComponent pixelRenderer = entity.GetComponent<PixelRendererComponent>();

                ComputeShader renderCompute = pixelRenderer.ChunkTextureCompute;
                
                // SAMPLERS
                renderCompute.SetTexture(0, "_Tex_Dirt_Course", 
                    AssetReferencer.Textures.Dirt_Course);
            
                // PARAMETERS
                renderCompute.SetInt("_TextureWidth", pixelData.Width);
                renderCompute.SetInt("_TextureHeight", pixelData.Height);
                renderCompute.SetVector("_WorldPosition", entity.transform.position);
                renderCompute.SetInt("_PPU", GameRenderData.PPU);
                pixelRenderer.PixelBuffer.SetData(pixelData.Pixels);
                renderCompute.SetBuffer(0, "_PixelBuffer", pixelRenderer.PixelBuffer);
            
                // OUTPUT
                renderCompute.SetTexture(0, "_ResultTexture", pixelRenderer.ResultTexture);
                
                renderCompute.Dispatch(0, 
                    Mathf.Max(pixelData.Width / 8, 1), 
                    Mathf.Max(pixelData.Height / 8, 1),
                    1);
            }
        }
    }
}