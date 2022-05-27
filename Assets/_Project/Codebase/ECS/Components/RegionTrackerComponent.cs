using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS.Components
{
    public sealed class RegionTrackerComponent : EntityComponent
    {
        [field: SerializeField] public List<Vector2Int> RegionsInsideOf { get; set; } = new List<Vector2Int>();
        [field: SerializeField] public bool UseColliderBounds { get; set; }
        [field: SerializeField] public Collider2D ColliderForBounds { get; set; }
    }
}