using PixelSim.ECS;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim
{
    public sealed class GameSimulator : MonoBehaviour
    {
        [SerializeField] private ECSManager _ecsManager;
        [SerializeField] private Transform _cameraTransform;

        private const float REGION_LOAD_DIST = 75f;

        private World _world;

        private void Awake()
        {
            _world = new World();
            
            LoadInitialRegions();
        }

        private void LateUpdate()
        {
            LoadRegionsAroundCamera();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            foreach (Region region in _world.Regions)
            {
                Gizmos.color = Color.green;
                foreach (Chunk chunk in region.chunks)
                    Gizmos.DrawWireCube(chunk.WorldSpaceCenter, Chunk.WorldSpaceSize);

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(region.WorldSpaceCenter, Region.WorldSpaceSize);
            }
        }

        private void LoadInitialRegions()
        {
            Vector2Int cameraRegionPos = SpaceConversions.WorldToRegion(_cameraTransform.position);

            for (int x = -2; x < 2; x++)
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int regionPos = cameraRegionPos + new Vector2Int(x, y);
                _world.CreateRegionAtPosition(regionPos);
            }
        }

        private void LoadRegionsAroundCamera()
        {
            Vector2Int cameraRegionPos = SpaceConversions.WorldToRegion(_cameraTransform.position);

            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int regionPos = cameraRegionPos + new Vector2Int(x, y);

                if (_world.HasRegionAtRegionPos(regionPos)) continue;

                Vector2 regionWorldPos = SpaceConversions.RegionToWorld(regionPos) + Region.WorldSpaceSize / 2f;
                float regionDistFromCamera = Vector2.Distance(_cameraTransform.position, regionWorldPos);

                if (regionDistFromCamera < REGION_LOAD_DIST)
                    _world.MoveExistingRegionToPosition(GetFurthestAvailableRegionFromCamera(), regionPos);
            }
        }
        
        private Region GetFurthestAvailableRegionFromCamera()
        {
            float furthestRegionDist = float.MinValue;
            Region furthestRegion = null;

            foreach (Region region in _world.Regions)
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