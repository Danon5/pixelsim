using System.Collections.Generic;
using UnityEngine;
using VoxelSim.Pooling;

namespace VoxelSim
{
    public sealed class World
    {
        public readonly Dictionary<Vector2Int, Region> regions = new Dictionary<Vector2Int, Region>();
        
        public Region CreateRegionAtPosition(in Vector2Int position)
        {
            Region region = RegionPool.RetrieveFromPool(position);
            regions.Add(region.position, region);

            return region;
        }

        public void DestroyRegionAtPosition(in Vector2Int position)
        {
            RegionPool.SendToPool(regions[position]);
            regions.Remove(position);
        }
    }
}