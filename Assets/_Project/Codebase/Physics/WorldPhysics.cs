using System.Collections.Generic;
using PixelSim.Pooling;
using UnityEngine;

namespace PixelSim.Physics
{
    public sealed class WorldPhysics : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkColliderPrefab;
        
        private static readonly Queue<Chunk> _chunksToRegenerate = new Queue<Chunk>();
        
        private readonly Dictionary<Chunk, ChunkCollider> _registeredChunks = new Dictionary<Chunk, ChunkCollider>();
        private readonly Dictionary<ChunkCollider, Coroutine> _activeRegenerations = new Dictionary<ChunkCollider, Coroutine>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _chunksToRegenerate.Clear();
        }
        
        private void LateUpdate()
        {
            while (_chunksToRegenerate.TryDequeue(out Chunk chunk))
                RegenerateChunk(chunk);
        }

        public static bool TryQueueChunkForRegeneration(Chunk chunk)
        {
            if (_chunksToRegenerate.Contains(chunk)) return false;
            
            _chunksToRegenerate.Enqueue(chunk);
            return true;
        }

        public void RegisterChunk(Chunk chunk)
        {
            ChunkCollider chunkCollider = ChunkColliderPool.RetrieveFromPool(chunk, this, _chunkColliderPrefab);
            _registeredChunks.Add(chunk, chunkCollider);
            TryQueueChunkForRegeneration(chunk);
        }
        
        public void DeregisterChunk(Chunk chunk)
        {
            ChunkColliderPool.SendToPool(_registeredChunks[chunk]);
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
        
        public void RegenerateChunk(Chunk chunk)
        {
            if (!_registeredChunks.TryGetValue(chunk, out ChunkCollider chunkCollider)) return;

            if (_activeRegenerations.TryGetValue(chunkCollider, out Coroutine regenerationRoutine))
            {
                if (chunkCollider.IsRegenerating)
                {
                    StopCoroutine(regenerationRoutine);
                    chunkCollider.DisposeRegeneration();
                    _activeRegenerations.Remove(chunkCollider);
                }
            }
            _activeRegenerations[chunkCollider] = StartCoroutine(chunkCollider.Regenerate());
        }
    }
}