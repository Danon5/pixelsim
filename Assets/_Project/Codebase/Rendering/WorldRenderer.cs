using System.Collections.Generic;
using PixelSim.Pooling;
using UnityEngine;

namespace PixelSim.Rendering
{
    public sealed class WorldRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkRendererPrefab;
        
        public const int PPU = 16;
        
        private static readonly Queue<Chunk> _chunksToRebuild = new Queue<Chunk>();
        
        private readonly Dictionary<Chunk, ChunkRenderer> _registeredChunks = new Dictionary<Chunk, ChunkRenderer>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _chunksToRebuild.Clear();
        }

        private void LateUpdate()
        {
            while (_chunksToRebuild.TryDequeue(out Chunk chunk))
                RebuildChunk(chunk);
        }

        public static bool TryQueueChunkForRebuild(Chunk chunk)
        {
            if (_chunksToRebuild.Contains(chunk)) return false;
            
            _chunksToRebuild.Enqueue(chunk);
            return true;
        }

        public void RegisterChunk(Chunk chunk)
        {
            ChunkRenderer chunkRenderer = ChunkRendererPool.RetrieveFromPool(chunk, this, _chunkRendererPrefab);
            chunkRenderer.Rebuild();
            
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

        public void RebuildChunk(Chunk chunk)
        {
            _registeredChunks[chunk].Rebuild();
        }
    }
}