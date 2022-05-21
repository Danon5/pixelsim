using System.Collections.Generic;
using UnityEngine;
using VoxelSim.Rendering;

namespace VoxelSim.Pooling
{
    public static class ChunkRendererPool
    {
        private static readonly Queue<ChunkRenderer> _chunkRendererPool = new Queue<ChunkRenderer>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _chunkRendererPool.Clear();
        }

        public static ChunkRenderer RetrieveFromPool(Chunk chunk, WorldRenderer worldRenderer, GameObject rendererPrefab)
        {
            ChunkRenderer renderer;

            if (_chunkRendererPool.Count > 0)
            {
                renderer = _chunkRendererPool.Dequeue();
                renderer.gameObject.SetActive(true);
            }
            else
                renderer = Object.Instantiate(rendererPrefab).GetComponent<ChunkRenderer>();

            renderer.transform.parent = worldRenderer.transform;
            renderer.transform.position = (Vector2)chunk.WorldSpaceOrigin;

            return renderer;
        }

        public static void SendToPool(ChunkRenderer renderer)
        {
            renderer.gameObject.SetActive(false);
            _chunkRendererPool.Enqueue(renderer);
        }
    }
}