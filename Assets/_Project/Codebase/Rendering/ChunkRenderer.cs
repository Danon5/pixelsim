using UnityEngine;

namespace VoxelSim.Rendering
{
    public sealed class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private ComputeShader _chunkTextureCompute;
        [SerializeField] private SpriteRenderer _solidRenderer;

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

            if (_solidRenderer.sprite == null)
                _solidRenderer.sprite = CreateChunkSprite();
            
            UpdateChunkTexture(_chunk, _solidRenderer.sprite.texture);
        }
        
        private Texture2D CreateChunkTexture()
        {
            Texture2D tex = new Texture2D(Chunk.SIZE, Chunk.SIZE, 
                TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            return tex;
        }

        private void UpdateChunkTexture(Chunk chunk, Texture2D texture)
        {
            _chunkTextureCompute.SetInt("ChunkSize", Chunk.SIZE);
            _voxelBuffer.SetData(chunk.voxels);
            _chunkTextureCompute.SetBuffer(0, "VoxelBuffer", _voxelBuffer);
            _chunkTextureCompute.SetTexture(0, "ResultTexture", _resultTexture);
            _chunkTextureCompute.Dispatch(0, 
                _resultTexture.width / 8, 
                _resultTexture.height / 8,
                1);

            Graphics.CopyTexture(_resultTexture, 0, texture, 0);
            //RenderTexture.active = _resultTexture;
            //texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            //RenderTexture.active = null;
        }

        private Sprite CreateChunkSprite()
        {
            Texture2D chunkTex = CreateChunkTexture();
            return Sprite.Create(chunkTex, new Rect(0f, 0f, chunkTex.width, chunkTex.height), 
                Vector2.zero, WorldRenderer.PPU, 2, SpriteMeshType.FullRect);
        }
    }
}