using PixelSim.ECS.Components;
using UnityEngine;

namespace PixelSim.ECS.Systems
{
    [CreateAssetMenu(
        fileName = nameof(PixelSimulationSystem), 
        menuName = "EntitySystems/" + nameof(PixelSimulationSystem))]
    public sealed class PixelSimulationSystem : EntitySystem
    {
        public override Archetype IterationArchetype { get; } = new Archetype(
            typeof(PixelDataComponent));

        public override void InitializeEntity(Entity entity)
        {
            base.InitializeEntity(entity);

            PixelDataComponent pixelData = entity.GetComponent<PixelDataComponent>();

            int size = pixelData.Width * pixelData.Height;
            
            pixelData.Pixels = new Pixel[size];

            for (int x = 0; x < pixelData.Width; x++)
            {
                for (int y = 0; y < pixelData.Height; y++)
                {
                    pixelData.Pixels[y * pixelData.Width + x] = new Pixel(PixelId.Dirt);
                }
            }
        }
    }
}