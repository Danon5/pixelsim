using UnityEngine;
using UnityEngine.UI;

namespace VoxelSim.Rendering
{
    public sealed class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private ComputeShader _chunkTextureCompute;
        [SerializeField] private RawImage _solidRenderImage;

        private ComputeBuffer _voxelBuffer;
        private RenderTexture _resultTexture;
        private Chunk _chunk;

        private void Awake()
        {
            _voxelBuffer = new ComputeBuffer(Chunk.SIZE * Chunk.SIZE, 4);
            
            _resultTexture = new RenderTexture(Chunk.SIZE, Chunk.SIZE, 32)
            {
                enableRandomWrite = true,
                useMipMap = false
            };
            _resultTexture.Create();

            _solidRenderImage.rectTransform.sizeDelta = Chunk.WorldSpaceSize;
            _solidRenderImage.texture = _resultTexture;
        }

        private void OnDestroy()
        {
            _voxelBuffer.Release();
            _resultTexture.Release();
        }

        public void AssignChunk(Chunk chunk)
        {
            _chunk = chunk;
        }

        public void Refresh()
        {
            if (_chunk == null) return;

            UpdateChunkTexture(_chunk);
        }

        private void UpdateChunkTexture(Chunk chunk)
        {
            _chunkTextureCompute.SetInt("ChunkSize", Chunk.SIZE);
            _voxelBuffer.SetData(chunk.voxels);
            _chunkTextureCompute.SetBuffer(0, "VoxelBuffer", _voxelBuffer);
            _chunkTextureCompute.SetTexture(0, "ResultTexture", _resultTexture);
            _chunkTextureCompute.Dispatch(0, 
                _resultTexture.width / 8, 
                _resultTexture.height / 8,
                1);
        }
    }
}