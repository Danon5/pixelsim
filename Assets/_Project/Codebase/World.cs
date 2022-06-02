using System.Collections.Generic;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim
{
    public sealed class World
    {
        public List<Region> Regions => _availableRegions;
        
        private readonly Dictionary<Vector2Int, Region> _activeRegions = new Dictionary<Vector2Int, Region>();
        private readonly List<Region> _availableRegions = new List<Region>();
        
        public static World Current { get; private set; }

        public World()
        {
            Current = this;
        }
        
        public bool TryGetRegionAtWorldPos(in Vector2 worldPos, out Region region)
        {
            return _activeRegions.TryGetValue(SpaceConversions.WorldToRegion(worldPos), out region);
        }

        public bool TryGetRegionAtRegionPos(in Vector2Int regionPos, out Region region)
        {
            return _activeRegions.TryGetValue(regionPos, out region);
        }

        public bool TryGetChunkAtWorldPos(in Vector2 worldPos, out Chunk chunk)
        {
            chunk = null;
            
            if (!TryGetRegionAtWorldPos(worldPos, out Region region)) return false;
            return region.TryGetChunkAtIndex(SpaceConversions.WorldToChunk(worldPos) - region.ChunkSpaceOrigin, out chunk);
        }

        public bool TryGetPixelAtWorldPos(in Vector2 worldPos, out Pixel pixel)
        {
            pixel = default;

            if (!TryGetChunkAtWorldPos(worldPos, out Chunk chunk)) return false;
            return chunk.TryGetPixelAtIndex(SpaceConversions.WorldToPixel(worldPos) - chunk.PixelSpaceOrigin, out pixel);
        }

        public void SetPixelAtPos(in Vector2 worldPos, in PixelId id)
        {
            if (!TryGetChunkAtWorldPos(worldPos, out Chunk chunk)) return;

            int index = IndexConversions.WorldToPixelIndex(worldPos, chunk);
            
            chunk.pixels[index].id = id;
        }

        public void SetPixelCircleAtPos(in Vector2 worldPos, in int pixelRadius, in PixelId id)
        {
            if (pixelRadius == 0) return;
            
            if (!TryGetChunkAtWorldPos(worldPos, out Chunk rootChunk)) return;

            Vector2Int rootIndexPos = IndexConversions.Index1DTo2D(
                IndexConversions.WorldToPixelIndex(worldPos, rootChunk), Chunk.SIZE);
            
            SetPixelCircleFromIndexPos(rootIndexPos, pixelRadius, rootChunk, id);
        }

        public void SetPixelCircleBetweenPoints(in Vector2 worldPos1, in Vector2 worldPos2, 
            in int pixelRadius, in PixelId id, in int spacing = 3)
        {
            
        }

        public bool HasRegionAtRegionPos(in Vector2Int regionPos)
        {
            return _activeRegions.ContainsKey(regionPos);
        }
        
        public void CreateRegionAtPosition(in Vector2Int position)
        {
            Region region = new Region(position);
            _activeRegions.Add(position, region);
            _availableRegions.Add(region);
        }

        public void MoveExistingRegionToPosition(Region region, in Vector2Int position)
        {
            _activeRegions.Remove(region.position);
            region.Initialize(position);
            _activeRegions.Add(region.position, region);
        }

        private void SetPixelCircleFromIndexPos(in Vector2Int rootIndexPos, in int pixelRadius, 
            in Chunk rootChunk, in PixelId id)
        {
            int sqrPixelRadius = pixelRadius * pixelRadius;
            
            for (int x = -pixelRadius; x <= pixelRadius; x++)
            for (int y = -pixelRadius; y <= pixelRadius; y++)
            {
                Vector2Int indexPos = rootIndexPos + new Vector2Int(x, y);

                float sqrDist = (rootIndexPos - indexPos).sqrMagnitude;

                if (sqrDist > sqrPixelRadius) continue;
                
                if (!TryGetValidPixelIndexFromRoot(indexPos, rootChunk, 
                    out Vector2Int validIndexPos, out Chunk validChunk)) continue;

                int index = IndexConversions.Index2DTo1D(validIndexPos, Chunk.SIZE);
                
                validChunk.pixels[index].id = id;
            }
        }

        private bool TryGetValidPixelIndexFromRoot(in Vector2Int rootPixelIndexPos, in Chunk rootChunk,
            out Vector2Int resultPixelIndexPos, out Chunk resultChunk)
        {
            resultPixelIndexPos = rootPixelIndexPos;
            resultChunk = rootChunk;
            
            if (IsValidPixelIndex(in rootPixelIndexPos))
                return true;

            Vector2 worldSpacePixelPos = SpaceConversions.PixelToWorld(rootPixelIndexPos, rootChunk);

            if (!TryGetChunkAtWorldPos(worldSpacePixelPos, out resultChunk))
                return false;

            resultPixelIndexPos =
                IndexConversions.Index1DTo2D(IndexConversions.WorldToPixelIndex(worldSpacePixelPos, resultChunk), Chunk.SIZE);

            return true;
        }

        private bool IsValidPixelIndex(in Vector2Int pixelIndexPos)
        {
            return
                pixelIndexPos.x >= 0 && pixelIndexPos.x < Chunk.SIZE &&
                pixelIndexPos.y >= 0 && pixelIndexPos.y < Chunk.SIZE;
        }
    }
}