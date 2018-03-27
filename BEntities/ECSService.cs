using System;
using System.Collections.Generic;
using BEntities.Components;

namespace BEntities
{
    internal class ECSService
    {
        /// <summary>
        /// Dictionary of registered entities
        /// </summary>
        internal Dictionary<int, Entity> Entities { get; set; } = new Dictionary<int, Entity>(128);

        private int _maxId = 0;

        private readonly Queue<int> _freeIds = new Queue<int>(128);

        /// <summary>
        /// Queue for processing adding of enitities
        /// </summary>
        internal Queue<Entity> EntitiesForAdding { get; } = new Queue<Entity>();

        /// <summary>
        /// Queue for processing removing of entities
        /// </summary>
        internal Queue<Entity> EntitiesForRemoving { get; } = new Queue<Entity>();


        internal event EntityEventHandler OnEntityAdded;

        internal event EntityEventHandler OnEntityRemoved;

        internal Queue<BaseComponent> ComponentsForRegistering { get; } = new Queue<BaseComponent>();

        internal Queue<BaseComponent> ComponentsForRemoving { get; } = new Queue<BaseComponent>();

        internal List<BaseComponentSystem> DrawSystems { get; set; } = new List<BaseComponentSystem>();

        internal List<BaseComponentSystem> UpdateSystems { get; set; } = new List<BaseComponentSystem>();

        internal Entity CreateEntity()
        {
            Entity entity = new Entity(this);

			if (_freeIds.Count != 0)
			{
				entity.Id = _freeIds.Dequeue();
			}
			else
			{
				entity.Id = ++_maxId;
			}

			EntitiesForAdding.Enqueue(entity);

			// it is necessary to attach default component - Transform2D
			entity.AttachComponent<Transform2DComponent>().Reset();

            return entity;
        }

        internal void DestroyEntity(Entity entity)
        {
			if (entity.MarkedToBeRemoved)
				return;

            foreach (KeyValuePair<Type, BaseComponent> entityAttachedComponent in entity.AttachedComponents)
            {
                DetachComponent(entity, entityAttachedComponent.Key);
            }

            entity.MarkedToBeRemoved = true;
            EntitiesForRemoving.Enqueue(entity);
        }

        /// <summary>
        /// Dispatches entity adding logic
        /// </summary>
        internal void ProcessEntitiesForAdding()
        {
            while (EntitiesForAdding.Count != 0)
            {
                Entity entity = EntitiesForAdding.Dequeue();

                Entities[entity.Id] = entity;

                OnEntityAdded?.Invoke(entity);
            }
        }

        /// <summary>
        /// Dispatches entity removing logic
        /// </summary>
        internal void ProcessEntitiesForRemoving()
        {
            while (EntitiesForRemoving.Count != 0)
            {
                Entity entity = EntitiesForRemoving.Dequeue();

                entity.AttachedComponents.Clear();
                _freeIds.Enqueue(entity.Id);

                Entities.Remove(entity.Id);

                OnEntityRemoved?.Invoke(entity);
                entity.MarkedToBeRemoved = false;
                entity.Service = null;
            } 
        }

        /// <summary>
        /// Dispatches component registering logic
        /// </summary>
        internal void ProcessComponentsForRegistering()
        {
            while (ComponentsForRegistering.Count != 0)
            {
                BaseComponent component = ComponentsForRegistering.Dequeue();

                foreach (BaseComponentProcessingSystem baseComponentSystem in DrawSystems)
                {
                    if (baseComponentSystem.IsAppliableComponent(component))
                    {
                        baseComponentSystem.RegisterComponent(component);
                    }
                }

                foreach (BaseComponentProcessingSystem baseComponentSystem in UpdateSystems)
                {
                    if (baseComponentSystem.IsAppliableComponent(component))
                    {
                        baseComponentSystem.RegisterComponent(component);
                    }
                }
            }
        }

        /// <summary>
        /// Dispatches components removing logic
        /// </summary>
        internal void ProcessComponentsForRemoving()
        {
            while (ComponentsForRemoving.Count != 0)
            {
                BaseComponent component = ComponentsForRemoving.Dequeue();

                foreach (BaseComponentProcessingSystem baseComponentSystem in DrawSystems)
                {
                    if (baseComponentSystem.IsAppliableComponent(component))
                    {
                        baseComponentSystem.UnregisterComponent(component);
                    }
                }

                foreach (BaseComponentProcessingSystem baseComponentSystem in UpdateSystems)
                {
                    if (baseComponentSystem.IsAppliableComponent(component))
                    {
                        baseComponentSystem.UnregisterComponent(component);
                    }
                }

				// perform manual resetting of internal caching variable for transform component
				if (component.GetType() == typeof(Transform2DComponent))
					component.SourceEntity._transform = null;

                component.SourceEntity.AttachedComponents.Remove(component.GetType());

                component.MarkedToBeRemoved = false;
                component.SourceEntity = null;
            } 
        }

        /// <summary>
        /// Performs attaching pre-routine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        internal T AttachComponent<T>(Entity entity) where T: BaseComponent, new()
        {
            T component = new T
            {
                SourceEntity = entity
            };

            entity.AttachedComponents[typeof(T)] = component;

            ComponentsForRegistering.Enqueue(component);

            return component;
        }

        internal void DetachComponent<T>(Entity entity) where T : BaseComponent
        {
            DetachComponent(entity, typeof(T));
        }

        internal void DetachComponent(Entity entity, Type componentType)
        {
            BaseComponent component = entity.AttachedComponents[componentType];
            component.MarkedToBeRemoved = true;

            ComponentsForRemoving.Enqueue(component);
        }

        /// <summary>
        /// Performs destroying of all entities immediately with releasing all resources
        /// </summary>
        internal void DestroyAllEntitiesImmediately()
        {
            foreach (KeyValuePair<int, Entity> keyValuePair in Entities)
            {
                DestroyEntity(keyValuePair.Value);
            }

            ProcessComponentsForRemoving();
            ProcessEntitiesForRemoving();
        }
	}
}
