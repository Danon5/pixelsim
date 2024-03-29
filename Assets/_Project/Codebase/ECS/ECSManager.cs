﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS
{
    public sealed class ECSManager : MonoBehaviour
    {
        [SerializeField] private List<EntitySystem> _initialSystems;
        
        private readonly Dictionary<Archetype, List<EntitySystem>> _systemGroups = new Dictionary<Archetype, List<EntitySystem>>();
        private readonly Dictionary<Archetype, List<Entity>> _entityGroups = new Dictionary<Archetype, List<Entity>>();

        private static ECSManager _singleton;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _singleton = null;
        }

        private void Awake()
        {
            _singleton = this;
            
            foreach (EntitySystem initialSystem in _initialSystems)
                InstantiateAndAddSystem(initialSystem);
        }

        private void Update()
        {
            foreach ((Archetype archetype, List<EntitySystem> systems) in _systemGroups)
            {
                foreach (EntitySystem system in systems)
                {
                    if (TryGetEntitiesWithArchetype(archetype, out List<Entity> entitiesWithArchetype))
                        system.Tick(entitiesWithArchetype);
                }
            }
        }
        
        private void FixedUpdate()
        {
            foreach ((Archetype archetype, List<EntitySystem> systems) in _systemGroups)
            {
                foreach (EntitySystem system in systems)
                {
                    if (TryGetEntitiesWithArchetype(archetype, out List<Entity> entitiesWithArchetype))
                        system.FixedTick(entitiesWithArchetype);
                }
            }
        }
        
        private void LateUpdate()
        {
            foreach ((Archetype archetype, List<EntitySystem> systems) in _systemGroups)
            {
                foreach (EntitySystem system in systems)
                {
                    if (TryGetEntitiesWithArchetype(archetype, out List<Entity> entitiesWithArchetype))
                        system.LateTick(entitiesWithArchetype);
                }
            }
        }

        public static void InstantiateAndAddSystem(in EntitySystem system)
        {
            Archetype iterationArchetype = system.IterationArchetype;
            
            if (!_singleton._entityGroups.ContainsKey(iterationArchetype))
                _singleton._entityGroups.Add(iterationArchetype, new List<Entity>());
            
            if (!_singleton._systemGroups.ContainsKey(iterationArchetype))
                _singleton._systemGroups.Add(iterationArchetype, new List<EntitySystem>());

            _singleton._systemGroups[iterationArchetype].Add(Instantiate(system));
        }

        public static Entity InstantiateEntity()
        {
            Entity newEntity = new GameObject("Entity").AddComponent<Entity>();
            
            newEntity.ComponentChangedEvent += _singleton.OnEntityComponentChanged;

            return newEntity;
        }
        
        public static Entity InstantiateEntityFromPrefab(GameObject prefab, Vector3 position = default, 
            Quaternion rotation = default, Transform parent = default)
        {
            Entity newEntity = Instantiate(prefab, position, rotation, parent).GetComponent<Entity>();

            foreach (Entity entity in newEntity.GetComponentsInChildren<Entity>())
            {
                entity.ComponentChangedEvent += _singleton.OnEntityComponentChanged;
                entity.AddExistingComponents();
            }
            
            return newEntity;
        }

        public static void DestroyEntity(Entity entity)
        {
            entity.ComponentChangedEvent -= _singleton.OnEntityComponentChanged;

            foreach (EntityComponent component in entity.components)
            {
                if (component is IDisposable disposable)
                    disposable.Dispose();
            }
            
            _singleton._entityGroups[entity.Archetype].Remove(entity);
            Destroy(entity.gameObject);
        }

        public static bool TryGetEntitiesWithArchetype(in Archetype archetype, out List<Entity> entitiesWithArchetype)
        {
            return _singleton._entityGroups.TryGetValue(archetype, out entitiesWithArchetype);
        }

        private void UpdateEntityArchetypeGroups(Entity entity)
        {
            foreach ((Archetype groupArchetype, var entityGroup) in _entityGroups)
            {
                bool containsEntity = entityGroup.Contains(entity);
                bool entityBelongsInGroup = entity.Archetype.Contains(groupArchetype);

                if (containsEntity && !entityBelongsInGroup)
                    entityGroup.Remove(entity);
                else if (!containsEntity && entityBelongsInGroup)
                {
                    entityGroup.Add(entity);

                    if (_systemGroups.TryGetValue(groupArchetype, out List<EntitySystem> systems))
                    {
                        foreach (EntitySystem system in systems)
                            system.InitializeEntity(entity);
                    }
                }
            }
        }
        
        private void OnEntityComponentChanged(object sender, Archetype oldArchetype)
        {
            if (!(sender is Entity entity)) return;
            
            UpdateEntityArchetypeGroups(entity);
        }
    }
}