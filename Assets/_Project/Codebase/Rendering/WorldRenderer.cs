using System.Collections.Generic;
using UnityEngine;
using VoxelSim.Pooling;

namespace VoxelSim.Rendering
{
    public sealed class WorldRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkRendererPrefab;
        
        public const int PPU = 16;
        
        private readonly Dictionary<Chunk, ChunkRenderer> _registeredChunks = new Dictionary<Chunk, ChunkRenderer>();

        public void RegisterChunk(Chunk chunk)
        {
            ChunkRenderer chunkRenderer = ChunkRendererPool.RetrieveFromPool(chunk, this, _chunkRendererPrefab);
            chunkRenderer.AssignChunk(chunk);
            chunkRenderer.Refresh();
            
            _registeredChunks.Add(chunk, chunkRenderer);
        }

        public void DeregisterChunk(Chunk chunk)
        {
            ChunkRendererPool.SendToPool(_registeredChunks[chunk]);
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
    }
}