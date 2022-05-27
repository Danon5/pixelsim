using System.Collections.Generic;
using PixelSim.ECS.Components;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(
        fileName = nameof(RegionEntityUnloadSystem), 
        menuName = "EntitySystems/" + nameof(RegionEntityUnloadSystem))]
    public sealed class RegionEntityUnloadSystem : EntitySystem
    {
        public override Archetype RequiredArchetype { get; } = new Archetype(
            typeof(RegionTrackerComponent), 
            typeof(UnloadOnRegionUnloadComponent));

        public override void LateTick(in List<Entity> entities)
        {
            base.LateTick(in entities);

            foreach (Entity entity in entities)
            {
                RegionTrackerComponent tracker = entity.GetComponent<RegionTrackerComponent>();
                
                foreach (Vector2Int regionPos in tracker.RegionsInsideOf)
                {
                    if (!World.Current.HasRegionAtRegionPos(regionPos))
                    {
                        entity.gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }
    }
}