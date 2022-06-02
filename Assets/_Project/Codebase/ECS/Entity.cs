using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.ECS
{
    public sealed class Entity : MonoBehaviour
    {
        public event EventHandler<Archetype> ComponentChangedEvent;

        public Archetype Archetype { get; } = new Archetype { componentTypes = new HashSet<Type>() };
        
        public readonly List<EntityComponent> components = new List<EntityComponent>();

        public T AddComponent<T>() where T : EntityComponent
        {
            T newComponent = gameObject.AddComponent<T>();
            components.Add(newComponent);
            Archetype oldArchetype = new Archetype();
            Archetype.componentTypes.Add(newComponent.GetType());
            
            ComponentChangedEvent?.Invoke(this, oldArchetype);

            return newComponent;
        }

        public new T GetComponent<T>() where T : EntityComponent
        {
            foreach (EntityComponent component in components)
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
            
            components.Remove(component);
            Archetype oldArchetype = new Archetype();
            Archetype.componentTypes.Remove(component.GetType());
            Destroy(component);
            
            ComponentChangedEvent?.Invoke(this, oldArchetype);
            
            if (component is IDisposable disposable)
                disposable.Dispose();
            
            return true;
        }

        public void AddExistingComponents()
        {
            Archetype oldArchetype = new Archetype();

            foreach (EntityComponent existingComponent in GetComponents<EntityComponent>())
            {
                components.Add(existingComponent);
                Archetype.componentTypes.Add(existingComponent.GetType());
            }

            ComponentChangedEvent?.Invoke(this, oldArchetype);
        }
    }
}