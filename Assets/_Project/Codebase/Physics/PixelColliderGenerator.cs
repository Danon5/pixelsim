using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PixelSim.Utility;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace PixelSim.Physics
{
    public sealed class PixelColliderGenerator
    {
        public JobHandle GenerationJobHandle { get; set; }
        public NativeArray<Pixel> NativePixels { get; set; }
        public NativeList<int> NativePathLengths { get; set; }
        public NativeList<Vector2> NativeVertices { get; set; }
        public NativeHashSet<Vector2Int> NativeCheckedIndices { get; set; }
        public bool IsGenerating { get; private set; }
        
        private const float PATH_SIMPLIFICATION_TOLERANCE = .05f;
        
        private CancellationToken _cancellationToken;

        public IEnumerator RegenerateRoutine(Pixel[] pixels, PolygonCollider2D collider, int width, int height)
        {
            IsGenerating = true;

            NativePixels = new NativeArray<Pixel>(pixels, Allocator.Persistent);
            NativePathLengths = new NativeList<int>(0, AllocatorManager.Persistent);
            NativeVertices = new NativeList<Vector2>(0, AllocatorManager.Persistent);
            NativeCheckedIndices = new NativeHashSet<Vector2Int>(0, AllocatorManager.Persistent);

            ColliderGenerationJob colliderGenerationJob = new ColliderGenerationJob
            {
                pixels = NativePixels,
                width = width,
                height = height,
                size = width * height,
                pathLengths = NativePathLengths,
                vertices = NativeVertices,
                checkedIndices = NativeCheckedIndices
            };

            GenerationJobHandle = colliderGenerationJob.Schedule();

            while (!GenerationJobHandle.IsCompleted)
                yield return null;

            GenerationJobHandle.Complete();

            collider.pathCount = NativePathLengths.Length;

            if (collider.pathCount == 0)
            {
                Dispose();
                yield break;
            }

            int maxLength = 0;

            foreach (int length in NativePathLengths)
            {
                if (length > maxLength)
                    maxLength = length;
            }
            
            List<Vector2> rawVertices = new List<Vector2>(maxLength);
            List<Vector2> simplifiedVertices = new List<Vector2>();
            int pathIndexOffset = 0;

            for (int i = 0; i < collider.pathCount; i++)
            {
                rawVertices.Clear();
                simplifiedVertices.Clear();

                int pathLength = NativePathLengths[i];
                
                for (int j = 0; j < pathLength; j++)
                    rawVertices.Add(NativeVertices[pathIndexOffset + j]);

                LineUtility.Simplify(rawVertices, PATH_SIMPLIFICATION_TOLERANCE, simplifiedVertices);
                collider.SetPath(i, simplifiedVertices);

                pathIndexOffset += pathLength;
            }

            Dispose();
        }

        public void Dispose()
        {
            if (!IsGenerating) return;
            
            GenerationJobHandle.Complete();
            NativePixels.Dispose();
            NativePathLengths.Dispose();
            NativeVertices.Dispose();
            NativeCheckedIndices.Dispose();
            IsGenerating = false;
        }

        [BurstCompile]
        private struct ColliderGenerationJob : IJob
        {
            [ReadOnly] public NativeArray<Pixel> pixels;
            [ReadOnly] public int width;
            [ReadOnly] public int height;
            [ReadOnly] public int size;

            [WriteOnly] public NativeList<int> pathLengths;
            [WriteOnly] public NativeList<Vector2> vertices;
            
            public NativeHashSet<Vector2Int> checkedIndices;

            public void Execute()
            {
                Vector2Int up = new Vector2Int(0, 1);
                Vector2Int down = new Vector2Int(0, -1);
                Vector2Int left = new Vector2Int(-1, 0);
                Vector2Int right = new Vector2Int(1, 0);
                Vector2Int one = new Vector2Int(1, 1);
                
                for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Vector2Int index = new Vector2Int(x, y);

                    if (checkedIndices.Contains(index)) continue;

                    if (IsSolidPixelIndex(index) && !IsSolidPixelIndex(index + left))
                        pathLengths.Add(PerformEdgeDetection(index, up, down, left, right, one));
                }
            }

            private int PerformEdgeDetection(in Vector2Int originIndex, in Vector2Int up, in Vector2Int down,
                in Vector2Int left, in Vector2Int right, in Vector2Int one)
            {
                Vector2Int currentIndex = originIndex;
                Vector2Int currentDirection = up;

                int maxIterations = size / 2;

                int pathLength = 0;

                do
                {
                    if (currentDirection == up || currentDirection == right)
                    {
                        // UP
                        if (IsSolidPixelIndex(currentIndex) &&
                            !IsSolidPixelIndex(currentIndex + left))
                            currentDirection = up;

                        // RIGHT
                        if (IsSolidPixelIndex(currentIndex + down) &&
                            !IsSolidPixelIndex(currentIndex))
                            currentDirection = right;

                        // DOWN
                        if (IsSolidPixelIndex(currentIndex - one) &&
                            !IsSolidPixelIndex(currentIndex + down))
                            currentDirection = down;

                        // LEFT
                        if (IsSolidPixelIndex(currentIndex + left) &&
                            !IsSolidPixelIndex(currentIndex - one))
                            currentDirection = left;
                    }
                    else
                    {
                        // DOWN
                        if (IsSolidPixelIndex(currentIndex - one) &&
                            !IsSolidPixelIndex(currentIndex + down))
                            currentDirection = down;

                        // LEFT
                        if (IsSolidPixelIndex(currentIndex + left) &&
                            !IsSolidPixelIndex(currentIndex - one))
                            currentDirection = left;

                        // UP
                        if (IsSolidPixelIndex(currentIndex) &&
                            !IsSolidPixelIndex(currentIndex + left))
                            currentDirection = up;

                        // RIGHT
                        if (IsSolidPixelIndex(currentIndex + down) &&
                            !IsSolidPixelIndex(currentIndex))
                            currentDirection = right;
                    }

                    vertices.Add((Vector2)currentIndex / GameRenderData.PPU);
                    checkedIndices.Add(currentIndex);
                    currentIndex += currentDirection;
                    pathLength++;
                    maxIterations--;
                } while (currentIndex != originIndex && maxIterations > 0);

                return pathLength;
            }

            private bool IsSolidPixelIndex(in Vector2Int index)
            {
                if (index.x < 0 || index.x >= width ||
                    index.y < 0 || index.y >= height)
                    return false;

                int nativeIndex = IndexConversions.Index2DTo1D(index, size);

                return pixels[nativeIndex].id != PixelId.Air;
            }
        }
    }
}