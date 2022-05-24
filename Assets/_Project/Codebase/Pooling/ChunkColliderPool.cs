using System.Collections.Generic;
using PixelSim.Physics;
using UnityEngine;

namespace PixelSim.Pooling
{
    public static class ChunkColliderPool
    {
        private static readonly Queue<ChunkCollider> _chunkColliderPool = new Queue<ChunkCollider>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _chunkColliderPool.Clear();
        }

        public static ChunkCollider RetrieveFromPool(Chunk chunk, WorldPhysics worldPhysics, GameObject colliderPrefab)
        {
            ChunkCollider collider;

            if (_chunkColliderPool.Count > 0)
            {
                collider = _chunkColliderPool.Dequeue();
                collider.Clear();
            }
            else
                collider = Object.Instantiate(colliderPrefab).GetComponent<ChunkCollider>();
            
            collider.AssignChunk(chunk);

            collider.transform.parent = worldPhysics.transform;
            collider.transform.position = (Vector2)chunk.WorldSpaceOrigin;

            return collider;
        }

        public static void SendToPool(ChunkCollider collider)
        {
            collider.Clear();
            _chunkColliderPool.Enqueue(collider);
        }
    }
}