using System.Collections.Generic;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim.Physics
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public sealed class ChunkCollider : MonoBehaviour
    {
        private const float PATH_SIMPLIFICATION_TOLERANCE = .05f;
        
        private readonly List<List<Vector2>> _vertexPaths = new List<List<Vector2>>();
        
        private PolygonCollider2D _collider;
        private Chunk _chunk;
        
        private void Awake()
        {
            _collider = GetComponent<PolygonCollider2D>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            foreach (List<Vector2> path in _vertexPaths)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    Vector2 pathNode = path[i];
                    Vector2 nextNode = i == path.Count - 1 ? path[0] : path[i + 1];
                    
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
            _vertexPaths.Clear();
        }

        public void Regenerate()
        {
            _vertexPaths.Clear();
            
            HashSet<Vector2Int> checkedIndices = new HashSet<Vector2Int>();
            List<List<Vector2>> rawPaths = new List<List<Vector2>>();

            for (int x = 0; x < Chunk.SIZE; x++)
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                Vector2Int index = new Vector2Int(x, y);

                if (checkedIndices.Contains(index)) continue;

                if (IsSolidPixelIndex(index) && !IsSolidPixelIndex(index + Vector2Int.left))
                {
                    rawPaths.Add(new List<Vector2>());
                    PerformEdgeDetection(index, checkedIndices, rawPaths[^1], true);
                }
                else if (!IsSolidPixelIndex(index) && IsSolidPixelIndex(index + Vector2Int.left))
                {
                    rawPaths.Add(new List<Vector2>());
                    PerformEdgeDetection(index, checkedIndices, rawPaths[^1], false);
                }
            }

            _collider.pathCount = rawPaths.Count;

            for (int i = 0; i < _collider.pathCount; i++)
            {
                CreateNewVertexPath().AddRange(rawPaths[i]);
                //LineUtility.Simplify(rawPaths[i], PATH_SIMPLIFICATION_TOLERANCE, CreateNewVertexPath());
                _collider.SetPath(i, _vertexPaths[i]);
            }
        }

        private void PerformEdgeDetection(in Vector2Int originIndex, 
            in HashSet<Vector2Int> checkedIndices, in List<Vector2> path, bool isOuter)
        {
            Vector2Int currentIndex = originIndex;
            Vector2Int currentDirection = Vector2Int.up;

            int maxIterations = Chunk.SQR_SIZE / 2;

            do
            {
                if (isOuter)
                {
                    // UP
                    if (IsSolidPixelIndex(currentIndex) &&
                        !IsSolidPixelIndex(currentIndex + Vector2Int.left))
                        currentDirection = Vector2Int.up;

                    // RIGHT
                    if (IsSolidPixelIndex(currentIndex + Vector2Int.down) &&
                        !IsSolidPixelIndex(currentIndex))
                        currentDirection = Vector2Int.right;

                    // DOWN
                    if (IsSolidPixelIndex(currentIndex - Vector2Int.one) &&
                        !IsSolidPixelIndex(currentIndex + Vector2Int.down))
                        currentDirection = Vector2Int.down;

                    // LEFT
                    if (IsSolidPixelIndex(currentIndex + Vector2Int.left) &&
                        !IsSolidPixelIndex(currentIndex - Vector2Int.one))
                        currentDirection = Vector2Int.left;
                }
                else
                {
                    // UP
                    if (!IsSolidPixelIndex(currentIndex) &&
                        IsSolidPixelIndex(currentIndex + Vector2Int.left))
                        currentDirection = Vector2Int.up;
                    
                    // RIGHT
                    if (!IsSolidPixelIndex(currentIndex + Vector2Int.down) &&
                        IsSolidPixelIndex(currentIndex))
                        currentDirection = Vector2Int.right;
                    
                    // DOWN
                    if (!IsSolidPixelIndex(currentIndex - Vector2Int.one) &&
                        IsSolidPixelIndex(currentIndex + Vector2Int.down))
                        currentDirection = Vector2Int.down;
                    
                    // LEFT
                    if (!IsSolidPixelIndex(currentIndex + Vector2Int.left) &&
                        IsSolidPixelIndex(currentIndex - Vector2Int.one))
                        currentDirection = Vector2Int.left;
                }

                path.Add(SpaceConversions.PixelToChunkLocal(currentIndex));
                checkedIndices.Add(currentIndex);
                currentIndex += currentDirection;

                maxIterations--;

            } while (currentIndex != originIndex && maxIterations > 0);
            
            
        }

        private List<Vector2> CreateNewVertexPath()
        {
            _vertexPaths.Add(new List<Vector2>());
            return _vertexPaths[^1];
        }

        public bool IsSolidPixelIndex(in Vector2Int index)
        {
            return _chunk.TryGetPixelAtIndex(index, out Pixel pixel) && pixel.id != PixelId.Air;
        }
    }
}