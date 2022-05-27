using System;
using System.Collections.Generic;
using PixelSim.Physics;
using PixelSim.Rendering;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim
{
    public sealed class World
    {
        public List<Region> Regions => _availableRegions;

        private readonly WorldRenderer _worldRenderer;
        private readonly WorldPhysics _worldPhysics;
        private readonly Transform _cameraTransform;
        private readonly Dictionary<Vector2Int, Region> _activeRegions = new Dictionary<Vector2Int, Region>();
        private readonly List<Region> _availableRegions = new List<Region>();
        
        public static World Current { get; private set; }

        public WorldRenderer Renderer => _worldRenderer;
        public WorldPhysics Physics => _worldPhysics;

        public World(WorldRenderer worldRenderer, WorldPhysics worldPhysics, Transform cameraTransform)
        {
            _worldRenderer = worldRenderer;
            _worldPhysics = worldPhysics;
            _cameraTransform = cameraTransform;
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

            PixelId originalId = chunk.pixels[index].id;
            chunk.pixels[index].id = id;

            if (originalId != id)
            {
                WorldRenderer.TryQueueChunkForRebuild(chunk);
                WorldPhysics.TryQueueChunkForRegeneration(chunk);
            }
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
        
        public void LoadRegionAtPosition(in Vector2Int position, in bool createNew)
        {
            if (!createNew && _availableRegions.Count == 0)
                throw new Exception("Trying to load pooled region when pool is empty.");
            
            Region region;

            if (createNew)
            {
                region = new Region(position);
                _worldRenderer.RegisterRegion(region);
                _worldPhysics.RegisterRegion(region);
                _activeRegions.Add(position, region);
                _availableRegions.Add(region);
            }
            else
            {
                region = GetFurthestAvailableRegionFromCamera();
                _worldRenderer.DeregisterRegion(region);
                _worldPhysics.DeregisterRegion(region);
                _activeRegions.Remove(region.position);
                region.Initialize(position);
                _worldRenderer.RegisterRegion(region);
                _worldPhysics.RegisterRegion(region);
                _activeRegions.Add(region.position, region);
            }
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
                
                PixelId originalId = validChunk.pixels[index].id;
                validChunk.pixels[index].id = id;

                if (originalId != id)
                {
                    WorldRenderer.TryQueueChunkForRebuild(validChunk);
                    WorldPhysics.TryQueueChunkForRegeneration(validChunk);
                }
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
        
        private Region GetFurthestAvailableRegionFromCamera()
        {
            float furthestRegionDist = float.MinValue;
            Region furthestRegion = null;

            foreach (Region region in _availableRegions)
            {
                float dist = Vector2.Distance(_cameraTransform.position, region.WorldSpaceCenter);

                if (dist > furthestRegionDist)
                {
                    furthestRegionDist = dist;
                    furthestRegion = region;
                }
            }

            return furthestRegion;
        }
    }
}