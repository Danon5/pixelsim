using System.Collections.Generic;
using UnityEngine;

namespace VoxelSim.Rendering
{
    public sealed class WorldRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkRendererPrefab;
        
        public const int PPU = 16;
        
        private readonly Dictionary<Chunk, ChunkRenderer> _registeredChunks = new Dictionary<Chunk, ChunkRenderer>();

        public void RegisterChunk(Chunk chunk)
        {
            ChunkRenderer chunkRenderer = CreateChunkRenderer(chunk);
            chunkRenderer.AssignChunk(chunk);
            chunkRenderer.Refresh();
            
            _registeredChunks.Add(chunk, chunkRenderer);
        }

        public void DeregisterChunk(Chunk chunk)
        {
            Destroy(_registeredChunks[chunk].gameObject);
            _registeredChunks.Remove(chunk);
        }

        public void RegisterRegion(Region region)
        {
            foreach (Chunk chunk in region.chunks)
                RegisterChunk(chunk);
        }
        
        public void DeregisterRegion(Region region)
        {
            foreach (Chunk chunk in region.chunks)
                DeregisterChunk(chunk);
        }

        public void RefreshChunk(Chunk chunk)
        {
            _registeredChunks[chunk].Refresh();
        }

        private ChunkRenderer CreateChunkRenderer(Chunk chunk)
        {
            return Instantiate(_chunkRendererPrefab, chunk.WorldSpaceCenter, 
                    Quaternion.identity, transform).GetComponent<ChunkRenderer>();
        }
    }
}