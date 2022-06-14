using System.Collections.Generic;
using System.Linq;
using PixelSim.Shared.ECS.Systems;
using Unity.Entities;
using UnityEngine;

namespace PixelSim.Shared.Gameplay
{
    public sealed class GameSimulation : MonoBehaviour
    {
        private List<SystemBase> _updateSystems;
        
        private void Awake()
        {
            _updateSystems = new List<SystemBase>
            {
                World.DefaultGameObjectInjectionWorld.CreateSystem<ChunkGenerationSystem>(),
                World.DefaultGameObjectInjectionWorld.CreateSystem<ChunkRendererSystem>()
            };
        }

        private void Update()
        {
            foreach (SystemBase system in _updateSystems.Where(system => system.ShouldRunSystem()))
                system.Update();
        }
    }
}