using Cysharp.Threading.Tasks;
using PixelSim.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PixelSim.WorldGeneration
{
    public static class WorldGenerator
    {
        private const float SEED_RANGE = 999999f;
        
        private static float _seed;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _seed = Random.Range(-SEED_RANGE, SEED_RANGE);
        }
        
        public static async UniTask GenerateChunk(Chunk chunk)
        {
            NativeArray<Pixel> pixels = new NativeArray<Pixel>(chunk.pixels, Allocator.Persistent);
            
            ChunkGenerationJob generationJob = new ChunkGenerationJob
            {
                chunkOrigin = chunk.WorldSpaceOrigin,
                seed = _seed,
                pixels = pixels
            };

            JobHandle jobHandle = generationJob.Schedule(pixels.Length, 16);

            while (!jobHandle.IsCompleted)
                await UniTask.Yield();
            
            jobHandle.Complete();
            
            pixels.CopyTo(chunk.pixels);

            pixels.Dispose();
        }

        [BurstCompile]
        public struct ChunkGenerationJob : IJobParallelFor
        {
            [ReadOnly] public Vector2 chunkOrigin;
            [ReadOnly] public float seed;
            
            public NativeArray<Pixel> pixels;

            public void Execute(int index)
            {
                Pixel pixel = pixels[index];

                int xIndex = index / Chunk.SIZE;
                int yIndex = index % Chunk.SIZE;

                float x = chunkOrigin.x + (float) xIndex / WorldRenderer.PPU;
                float y = chunkOrigin.y + (float) yIndex / WorldRenderer.PPU;

                float perlinResult1 = Mathf.PerlinNoise(seed + x * .25f, seed + y * .25f);
                float perlinResult2 = Mathf.PerlinNoise(seed + x * .75f, seed + y * .75f) * .15f;

                float finalResult = perlinResult1 + perlinResult2;

                pixel.id = finalResult > .5f ? PixelId.Dirt : PixelId.Air;
                
                pixels[index] = pixel;
            }
        }
    }
}