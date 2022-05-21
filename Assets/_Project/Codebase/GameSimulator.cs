using System.Collections.Generic;
using UnityEngine;
using VoxelSim.ECS;
using VoxelSim.Rendering;
using VoxelSim.Utility;

namespace VoxelSim
{
    public sealed class GameSimulator : MonoBehaviour
    {
        [SerializeField] private ECSManager _ecsManager;
        [SerializeField] private WorldRenderer _worldRenderer;
        [SerializeField] private Transform _cameraTransform;

        private const float REGION_LOAD_DIST = 75f;

        private readonly Dictionary<Vector2Int, Region> _activeRegions = new Dictionary<Vector2Int, Region>();
        private readonly Queue<Vector2Int> _regionsToDestroy = new Queue<Vector2Int>();
        
        private World _world;

        private void Awake()
        {
            _world = new World();
        }

        private void LateUpdate()
        {
            LoadRegionsAroundCamera();
        }

        private void LoadRegionsAroundCamera()
        {
            Vector2Int cameraRegionPos = SpaceConversions.WorldToRegion(_cameraTransform.position);

            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int regionPos = cameraRegionPos + new Vector2Int(x, y);

                if (HasRegionAtPosition(regionPos)) continue;

                Vector2 regionWorldPos = SpaceConversions.RegionToWorld(regionPos) + Region.WorldSpaceSize / 2f;
                float regionDistFromCamera = Vector2.Distance(_cameraTransform.position, regionWorldPos);

                if (regionDistFromCamera < REGION_LOAD_DIST)
                    LoadRegionAtPosition(regionPos);
            }

            foreach (KeyValuePair<Vector2Int, Region> regionEntry in _activeRegions)
            {
                Vector2 regionWorldPos = SpaceConversions.RegionToWorld(regionEntry.Key) + Region.WorldSpaceSize / 2f;
                float regionDistFromCamera = Vector2.Distance(_cameraTransform.position, regionWorldPos);

                if (regionDistFromCamera > REGION_LOAD_DIST)
                    _regionsToDestroy.Enqueue(regionEntry.Key);
            }

            while (_regionsToDestroy.TryDequeue(out Vector2Int regionPosition))
                UnloadRegionAtPosition(regionPosition);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            foreach (KeyValuePair<Vector2Int, Region> regionEntry in _world.regions)
            {
                Gizmos.color = Color.green;
                foreach (Chunk chunk in regionEntry.Value.chunks)
                    Gizmos.DrawWireCube(chunk.WorldSpaceCenter, Chunk.WorldSpaceSize);
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(regionEntry.Value.WorldSpaceCenter, Region.WorldSpaceSize);
            }
        }

        private bool HasRegionAtPosition(in Vector2Int position)
        {
            return _activeRegions.ContainsKey(position);
        }

        private void LoadRegionAtPosition(in Vector2Int position)
        {
            Region region = _world.CreateRegionAtPosition(position);
            _worldRenderer.RegisterRegion(region);
            _activeRegions.Add(position, region);
        }

        private void UnloadRegionAtPosition(in Vector2Int position)
        {
            Region region = _activeRegions[position];
            _world.DestroyRegionAtPosition(position);
            _worldRenderer.DeregisterRegion(region);
            _activeRegions.Remove(position);
        }
    }
}