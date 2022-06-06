using System;
using System.Collections.Generic;
using System.Linq;
using PixelSim.Gameplay.ECS.BufferElements;
using PixelSim.Gameplay.ECS.Components;
using PixelSim.Gameplay.ECS.Systems;
using PixelSim.Gameplay.ECS.Tags;
using Unity.Entities;
using UnityEngine;

namespace PixelSim.Gameplay
{
    public sealed class GameSimulation : MonoBehaviour
    {
        private List<SystemBase> _updateSystems;
        
        private void Awake()
        {
            _updateSystems = new List<SystemBase>
            {
                World.DefaultGameObjectInjectionWorld.CreateSystem<ChunkGenerationSystem>()
            };
            
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(
                typeof(IntPositionComponent),
                typeof(ChunkPixelBufferElement),
                typeof(ChunkRequiresInitializationTag));
        }

        private void Update()
        {
            foreach (SystemBase system in _updateSystems.Where(system => system.ShouldRunSystem()))
                system.Update();
        }
    }
}