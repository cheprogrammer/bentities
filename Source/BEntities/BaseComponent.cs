using System;
using BEntities.Components;

namespace BEntities
{
    public abstract class BaseComponent
    {
        /// <summary>
        /// Gets the source entity
        /// </summary>
        public Entity SourceEntity { get; internal set; }

        public Transform2DComponent Transform => SourceEntity.Transform;

        public bool MarkedToBeRemoved { get; internal set; } = false;

        /// <summary>
        /// Performs initialization of Component right before registering it in systems
        /// </summary>
        public virtual void Initialize()
        {

        }

        public virtual void Reset()
        {
            
        }

        /// <summary>
        /// Gets the component of type <typeparamref  name="T"/> from current entity or default value (if component does not exist)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponentOrDefault<T>() where T: BaseComponent
        {
            return SourceEntity.GetComponentOrDefault<T>();
        }

        /// <summary>
        /// Gets the component of type <typeparamref  name="T"/> from current entity. If component does not exists, throws an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            return SourceEntity.GetComponentOrDefault<T>() ?? throw new ArgumentException($"Component '{typeof(T).Name}' does not belong to current entity");
        }

        /// <summary>
        ///  Gets the component of type <typeparamref  name="T"/> from current entity or first parent entity. Returns default value if component does not exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponentInParentOrDefault<T>() where T : BaseComponent
        {
            return SourceEntity.GetComponentInParentDefault<T>();
        }

        /// <summary>
        ///  Gets the component of type <typeparamref  name="T"/> from current entity or first parent entity. If component does not exists, throws an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponentInParent<T>() where T : BaseComponent
        {
            return SourceEntity.GetComponentInParentDefault<T>() ?? throw new ArgumentException($"Component '{typeof(T).Name}' does not belong to current entity or any other parent entities");
        }
    }
}
