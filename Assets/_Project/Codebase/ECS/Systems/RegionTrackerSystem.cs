using System.Collections.Generic;
using PixelSim.ECS.Components;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(
        fileName = nameof(RegionTrackerSystem), 
        menuName = "EntitySystems/" + nameof(RegionTrackerSystem))]
    public sealed class RegionTrackerSystem : EntitySystem
    {
        public override Archetype IterationArchetype => new Archetype(
            typeof(RegionTrackerComponent));

        public override void LateTick(in List<Entity> entities)
        {
            base.LateTick(in entities);

            foreach (Entity entity in entities)
            {
                RegionTrackerComponent tracker = entity.GetComponent<RegionTrackerComponent>();

                tracker.RegionsInsideOf.Clear();
                tracker.RegionsInsideOf.Add(SpaceConversions.WorldToRegion(entity.transform.position));
            }
        }
    }
}