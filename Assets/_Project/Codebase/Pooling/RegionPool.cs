using System.Collections.Generic;
using UnityEngine;

namespace VoxelSim.Pooling
{
    public static class RegionPool
    {
        private static readonly Queue<Region> _regionPool = new Queue<Region>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _regionPool.Clear();
        }

        public static Region RetrieveFromPool(in Vector2Int position)
        {
            Region region;
            
            if (_regionPool.Count > 0)
            {
                region = _regionPool.Dequeue();
                region.Initialize(position);
            }
            else
                region = new Region(position);
            
            return region;
        }

        public static void SendToPool(Region region)
        {
            _regionPool.Enqueue(region);
        }
    }
}