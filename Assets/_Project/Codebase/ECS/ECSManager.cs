using System.Collections.Generic;
using PixelSim.ECS.Systems;
using UnityEngine;

namespace PixelSim.ECS
{
    public sealed class ECSManager : MonoBehaviour
    {
        [SerializeField] private List<BaseEntitySystem> _systems;
        
        private Dictionary<Archetype, List<Entity>> _entities = new Dictionary<Archetype, List<Entity>>();

        private void Update()
        {
            foreach (BaseEntitySystem system in _systems)
            {
                if (_entities.TryGetValue(system.RequiredArchetype, out List<Entity> entitiesWithArchetype))
                    system.Tick(entitiesWithArchetype);
            }
        }
        
        private void FixedUpdate()
        {
            foreach (BaseEntitySystem system in _systems)
            {
                if (_entities.TryGetValue(system.RequiredArchetype, out List<Entity> entitiesWithArchetype))
                    system.Tick(entitiesWithArchetype);
            }
        }
        
        private void LateUpdate()
        {
            foreach (BaseEntitySystem system in _systems)
            {
                if (_entities.TryGetValue(system.RequiredArchetype, out List<Entity> entitiesWithArchetype))
                    system.Tick(entitiesWithArchetype);
            }
        }

        public void InstantiateAndAddSystem(BaseEntitySystem system)
        {
            _systems.Add(Instantiate(system));
        }
    }
}