using System;
using System.Collections;
using System.Collections.Generic;
using PixelSim.Rendering;
using PixelSim.Utility;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PixelSim.Physics
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public sealed class ChunkCollider : MonoBehaviour
    {
        public bool IsRegenerating { get; private set; }
        
        private const float PATH_SIMPLIFICATION_TOLERANCE = .05f;

        private PolygonCollider2D _collider;
        private Chunk _chunk;
        
        private NativeArray<Pixel> _nativePixels;
        private NativeList<int> _nativePathLengths;
        private NativeList<Vector2> _nativeVertices;
        private NativeHashSet<Vector2Int> _nativeCheckedIndices;

        private void Awake()
        {
            _collider = GetComponent<PolygonCollider2D>();
        }

        private void OnDestroy()
        {
            DisposeRegeneration(); 
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < _collider.pathCount; i++)
            {
                Vector2[] path = _collider.GetPath(i);

                for (int j = 0; j < path.Length; j++)
                {
                    Vector2 pathNode = path[j];
                    Vector2 nextNode = j == path.Length - 1 ? path[0] : path[j + 1];

                    Gizmos.DrawLine(
                        (Vector2) transform.position + pathNode,
                        (Vector2) transform.position + nextNode);
                }
            }
        }

        public void AssignChunk(in Chunk chunk)
        {
            _chunk = chunk;
        }

        public void Clear()
        {
            _collider.pathCount = 0;
        }

        public IEnumerator Regenerate()
        {
            IsRegenerating = true;

            _nativePixels = new NativeArray<Pixel>(_chunk.pixels, Allocator.Persistent);
            _nativePathLengths = new NativeList<int>(0, AllocatorManager.Persistent);
            _nativeVertices = new NativeList<Vector2>(0, AllocatorManager.Persistent);
            _nativeCheckedIndices = new NativeHashSet<Vector2Int>(0, AllocatorManager.Persistent);

            ColliderGenerationJob colliderGenerationJob = new ColliderGenerationJob
            {
                pixels = _nativePixels,
                pathLengths = _nativePathLengths,
                vertices = _nativeVertices,
                checkedIndices = _nativeCheckedIndices
            };

            JobHandle jobHandle = colliderGenerationJob.Schedule();

            while (!jobHandle.IsCompleted)
                yield return null;

            jobHandle.Complete();

            _collider.pathCount = _nativePathLengths.Length;

            if (_collider.pathCount == 0)
            {
                DisposeRegeneration();
                yield break;
            }

            int maxLength = 0;

            foreach (int length in _nativePathLengths)
            {
                if (length > maxLength)
                    maxLength = length;
            }
            
            List<Vector2> rawVertices = new List<Vector2>(maxLength);
            List<Vector2> simplifiedVertices = new List<Vector2>();
            int pathIndexOffset = 0;

            for (int i = 0; i < _collider.pathCount; i++)
            {
                rawVertices.Clear();
                simplifiedVertices.Clear();

                int pathLength = _nativePathLengths[i];
                
                for (int j = 0; j < pathLength; j++)
                    rawVertices.Add(_nativeVertices[pathIndexOffset + j]);

                LineUtility.Simplify(rawVertices, PATH_SIMPLIFICATION_TOLERANCE, simplifiedVertices);
                _collider.SetPath(i, simplifiedVertices);

                pathIndexOffset += pathLength;
            }

            DisposeRegeneration();
        }

        public void DisposeRegeneration()
        {
            if (!IsRegenerating) return;
            
            _nativePixels.Dispose();
            _nativePathLengths.Dispose();
            _nativeVertices.Dispose();
            _nativeCheckedIndices.Dispose();
            IsRegenerating = false;
        }

        [BurstCompile]
        private struct ColliderGenerationJob : IJob
        {
            [ReadOnly] public NativeArray<Pixel> pixels;

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
                
                for (int x = 0; x < Chunk.SIZE; x++)
                for (int y = 0; y < Chunk.SIZE; y++)
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

                int maxIterations = Chunk.SQR_SIZE / 2;

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

                    vertices.Add((Vector2)currentIndex / WorldRenderer.PPU);
                    checkedIndices.Add(currentIndex);
                    currentIndex += currentDirection;
                    pathLength++;
                    maxIterations--;
                } while (currentIndex != originIndex && maxIterations > 0);

                return pathLength;
            }

            private bool IsSolidPixelIndex(in Vector2Int index)
            {
                if (index.x < 0 || index.x >= Chunk.SIZE ||
                    index.y < 0 || index.y >= Chunk.SIZE)
                    return false;

                int nativeIndex = IndexConversions.Index2DTo1D(index, Chunk.SIZE);

                return pixels[nativeIndex].id != PixelId.Air;
            }
        }
    }
}