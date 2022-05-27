using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS
{
    public sealed class Entity : MonoBehaviour
    {
        public event EventHandler<Archetype> ComponentChangedEvent;

        public Archetype Archetype { get; } = new Archetype { componentTypes = new HashSet<Type>() };
        
        private readonly List<EntityComponent> _components = new List<EntityComponent>();

        private void Awake()
        {
            AddExistingComponents();
        }

        public T AddComponent<T>() where T : EntityComponent
        {
            T newComponent = gameObject.AddComponent<T>();
            _components.Add(newComponent);
            Archetype oldArchetype = new Archetype();
            Archetype.componentTypes.Add(newComponent.GetType());
            
            ComponentChangedEvent?.Invoke(this, oldArchetype);

            return newComponent;
        }

        public new T GetComponent<T>() where T : EntityComponent
        {
            foreach (EntityComponent component in _components)
            {
                if (component is T tComponent)
                    return tComponent;
            }

            return null;
        }

        public new bool TryGetComponent<T>(out T component) where T : EntityComponent
        {
            component = GetComponent<T>();
            
            return component != null;
        }

        public bool RemoveComponent<T>() where T : EntityComponent
        {
            if (!TryGetComponent(out T component)) return false;
            
            _components.Remove(component);
            Archetype oldArchetype = new Archetype();
            Archetype.componentTypes.Remove(component.GetType());
            Destroy(component);
            
            ComponentChangedEvent?.Invoke(this, oldArchetype);
            
            return true;
        }

        private void AddExistingComponents()
        {
            Archetype oldArchetype = new Archetype();

            foreach (EntityComponent existingComponent in GetComponents<EntityComponent>())
            {
                _components.Add(existingComponent);
                Archetype.componentTypes.Add(existingComponent.GetType());
            }

            ComponentChangedEvent?.Invoke(this, oldArchetype);
        }
    }
}