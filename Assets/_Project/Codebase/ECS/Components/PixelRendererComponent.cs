using System;
using UnityEngine;
using UnityEngine.UI;

namespace PixelSim.ECS.Components
{
    public sealed class PixelRendererComponent : EntityComponent, IDisposable
    {
        [field: SerializeField] public ComputeShader ChunkTextureCompute { get; set; }
        [field: SerializeField] public RawImage SolidRenderImage { get; set; }
        public ComputeBuffer PixelBuffer { get; set; }
        public RenderTexture ResultTexture { get; set; }

        public void Dispose()
        {
            PixelBuffer?.Release();
            if (ResultTexture != null)
                ResultTexture.Release();
        }
    }
}