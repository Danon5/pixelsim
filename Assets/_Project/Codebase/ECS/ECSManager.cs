using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS
{
    public sealed class ECSManager : MonoBehaviour
    {
        [SerializeField] private List<EntitySystem> _initialSystems;
        
        private readonly List<EntitySystem> _activeSystems = new List<EntitySystem>();
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
            foreach (EntitySystem system in _activeSystems)
            {
                if (_entityGroups.TryGetValue(system.RequiredArchetype, out List<Entity> entitiesWithArchetype))
                    system.Tick(entitiesWithArchetype);
            }
        }
        
        private void FixedUpdate()
        {
            foreach (EntitySystem system in _activeSystems)
            {
                if (_entityGroups.TryGetValue(system.RequiredArchetype, out List<Entity> entitiesWithArchetype))
                    system.FixedTick(entitiesWithArchetype);
            }
        }
        
        private void LateUpdate()
        {
            foreach (EntitySystem system in _activeSystems)
            {
                if (_entityGroups.TryGetValue(system.RequiredArchetype, out List<Entity> entitiesWithArchetype))
                    system.LateTick(entitiesWithArchetype);
            }
        }

        public static void InstantiateAndAddSystem(EntitySystem system)
        {
            if (!_singleton._entityGroups.ContainsKey(system.RequiredArchetype))
                _singleton._entityGroups.Add(system.RequiredArchetype, new List<Entity>());
            _singleton._activeSystems.Add(Instantiate(system));
        }

        public static Entity CreateEntity()
        {
            Entity newEntity = new GameObject("Entity").AddComponent<Entity>();

            _singleton.AddEntityToArchetypeGroups(newEntity);
            newEntity.ComponentChangedEvent += _singleton.OnEntityComponentChanged;

            return newEntity;
        }
        
        public static Entity CreateEntityFromPrefab(GameObject prefab, Vector3 position = default, 
            Quaternion rotation = default, Transform parent = default)
        {
            Entity newEntity = Instantiate(prefab, position, rotation, parent).GetComponent<Entity>();

            foreach (Entity entity in newEntity.GetComponentsInChildren<Entity>())
            {
                _singleton.AddEntityToArchetypeGroups(entity);
                entity.ComponentChangedEvent += _singleton.OnEntityComponentChanged;
            }
            
            return newEntity;
        }

        public static void DestroyEntity(Entity entity)
        {
            entity.ComponentChangedEvent -= _singleton.OnEntityComponentChanged;
            
            Destroy(entity.gameObject);
        }

        private void AddEntityToArchetypeGroups(Entity entity)
        {
            foreach ((Archetype groupArchetype, var entityGroup) in _entityGroups)
            {
                bool entityBelongsInGroup = entity.Archetype.Contains(groupArchetype);
                if (entityBelongsInGroup)
                    entityGroup.Add(entity);
            }
        }
        
        private void OnEntityComponentChanged(object sender, Archetype oldArchetype)
        {
            if (!(sender is Entity entity)) return;
            
            foreach ((Archetype groupArchetype, var entityGroup) in _entityGroups)
            {
                bool containsEntity = entityGroup.Contains(entity);
                bool entityBelongsInGroup = entity.Archetype.Contains(groupArchetype);
                
                if (containsEntity && !entityBelongsInGroup)
                    entityGroup.Remove(entity);
                if (!containsEntity && entityBelongsInGroup)
                    entityGroup.Add(entity);
            }
        }
    }
}