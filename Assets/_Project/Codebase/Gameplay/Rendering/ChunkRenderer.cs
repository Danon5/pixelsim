using System;
using PixelSim.Gameplay.ECS.BufferElements;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PixelSim.Gameplay.Rendering
{
    public sealed class ChunkRenderer : MonoBehaviour
    {
        [Header("Compute Shaders")]
        [SerializeField] private ComputeShader _chunkTextureCompute;
        [Space]
        [Header("Render Images")]
        [SerializeField] private RawImage _solidRenderImage;
        [SerializeField] private RawImage _liquidRenderImage;

        private ComputeBuffer _pixelBuffer;
        private RenderTexture _solidRenderTexture;
        private RenderTexture _liquidRenderTexture;

        private void Awake()
        {
            const int size = GameConstants.CHUNK_SIZE;
            const int sqr_size = GameConstants.CHUNK_SIZE_SQR;
            const int stride = 4;

            _pixelBuffer = new ComputeBuffer(sqr_size, stride, ComputeBufferType.Structured);
            _solidRenderTexture = CreateRenderTexture(size, size);
            _liquidRenderTexture = CreateRenderTexture(size, size);
            _solidRenderImage.texture = _solidRenderTexture;
            _liquidRenderImage.texture = _liquidRenderTexture;
            
            Vector2 renderImageSizeDelta = new Vector2(size, size) / GameConstants.PPU;
            _solidRenderImage.rectTransform.sizeDelta = renderImageSizeDelta;
            _liquidRenderImage.rectTransform.sizeDelta = renderImageSizeDelta;
        }

        private void OnDestroy()
        {
            _pixelBuffer.Release();
            _solidRenderTexture.Release();
            _liquidRenderTexture.Release();
        }

        public void UpdateRenderTextures(in NativeArray<ChunkPixelBufferElement> pixelBuffer)
        {
            _pixelBuffer.SetData(pixelBuffer);
            
            _chunkTextureCompute.SetInt("ChunkSize", GameConstants.CHUNK_SIZE);
            _chunkTextureCompute.SetBuffer(0, "PixelBuffer", _pixelBuffer);
            _chunkTextureCompute.SetTexture(0, "SolidRenderTexture", _solidRenderTexture);
            
            _chunkTextureCompute.Dispatch(0,
                Mathf.Max(GameConstants.CHUNK_SIZE / 8, 1),
                Mathf.Max(GameConstants.CHUNK_SIZE / 8, 1),
                1);
        }

        private RenderTexture CreateRenderTexture(in int width, in int height)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 16)
            {
                enableRandomWrite = true,
                useMipMap = false,
                filterMode = FilterMode.Point
            };
            renderTexture.Create();

            return renderTexture;
        }
    }
}