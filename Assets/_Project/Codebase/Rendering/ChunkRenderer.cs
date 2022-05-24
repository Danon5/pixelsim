using PixelSim.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace PixelSim.Rendering
{
    public sealed class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private ComputeShader _chunkTextureCompute;
        [SerializeField] private RawImage _solidRenderImage;

        private ComputeBuffer _pixelBuffer;
        private RenderTexture _resultTexture;
        private Chunk _chunk;

        private void Awake()
        {
            _pixelBuffer = new ComputeBuffer(Chunk.SQR_SIZE, 4);
            
            _resultTexture = new RenderTexture(Chunk.SIZE, Chunk.SIZE, 32)
            {
                enableRandomWrite = true,
                useMipMap = false,
                filterMode = FilterMode.Point
            };
            _resultTexture.Create();

            _solidRenderImage.rectTransform.sizeDelta = Chunk.WorldSpaceSize;
            _solidRenderImage.texture = _resultTexture;
        }

        private void OnDestroy()
        {
            _pixelBuffer.Release();
            _resultTexture.Release();
        }

        public void SetRendererActive(bool active)
        {
            _solidRenderImage.enabled = active;
        }

        public void AssignChunk(Chunk chunk)
        {
            _chunk = chunk;
        }

        public void Rebuild()
        {
            if (_chunk == null) return;

            UpdateChunkTexture(_chunk);
        }

        private void UpdateChunkTexture(Chunk chunk)
        {
            // SAMPLERS
            _chunkTextureCompute.SetTexture(0, "_Tex_Dirt_Course", AssetReferencer.Textures.Dirt_Course);
            
            // PARAMETERS
            _chunkTextureCompute.SetInt("_ChunkSize", Chunk.SIZE);
            _chunkTextureCompute.SetVector("_ChunkPos", new Vector4(chunk.position.x, chunk.position.y, 0f, 0f));
            _chunkTextureCompute.SetInt("_PPU", WorldRenderer.PPU);
            _pixelBuffer.SetData(chunk.pixels);
            _chunkTextureCompute.SetBuffer(0, "_PixelBuffer", _pixelBuffer);
            
            // OUTPUT
            _chunkTextureCompute.SetTexture(0, "_ResultTexture", _resultTexture);
            
            _chunkTextureCompute.Dispatch(0, 
                _resultTexture.width / 8, 
                _resultTexture.height / 8,
                1);
        }
    }
}