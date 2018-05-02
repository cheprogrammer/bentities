using System;
using System.Collections.Generic;
using BEntities.Components;

namespace BEntities
{
    public class Entity
    {
        public int Id { get; internal set; }

        public string Name { get; set; } = "Entity";

        internal ECSService Service { get; set; }

		// used for caching of transform components
		internal Transform2DComponent _transform = null;

        public Transform2DComponent Transform
        {
            get
            {
				if (_transform != null)
					return _transform;

                BaseComponent result;

                if (AttachedComponents.TryGetValue(typeof(Transform2DComponent), out result))
                    return (_transform = (Transform2DComponent)result);

                return null;
            }
        }

        internal Dictionary<Type, BaseComponent> AttachedComponents { get; set; } = new Dictionary<Type, BaseComponent>();

        public bool MarkedToBeRemoved { get; internal set; } = false;

        public bool IsActive { get; set; } = true;

        internal Entity(ECSService sourceService)
        {
            Service = sourceService;
        }

		/// <summary>
		/// Attaches component of specified type to current entity
		/// This component will be attached at the start of next update procedure
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public T AttachComponent<T>() where T : BaseComponent, new()
        {
            return Service.AttachComponent<T>(this);
        }

		/// <summary>
		/// Detaches component from current entity
		/// This component will be removed at the end of current update procedure
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void DetachComponent<T>() where T : BaseComponent
        {
            DetachComponent(typeof(T));
        }

        /// <summary>
        /// Detaches component from current entity
		/// This component will be removed at the end of current update procedure
        /// </summary>
        /// <param name="componentType"></param>
        public void DetachComponent(Type componentType)
        {
            Service.DetachComponent(this, componentType);
        }

        public IEnumerable<BaseComponent> GetComponents()
        {
            return AttachedComponents.Values;
        }

		/// <summary>
		/// Gets attached Component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>Attached component of specified Type or Null if there is no attached component of specified Type</returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            BaseComponent result;

            if (AttachedComponents.TryGetValue(typeof(T), out result))
            {
                return (T) result;
            }

            return null;
        }

        public T GetComponentInParent<T>() where T : BaseComponent
        {
            BaseComponent result = GetComponent<T>();

            if (result == null)
            {
                result = Transform.Parent.SourceEntity.GetComponentInParent<T>();

                if (result != null)
                    return (T) result;
            }

            return null;
        }

		/// <summary>
		/// Destroy current entity
		/// All components of current entity will be detached and destroyed at the end of current update procedure
		/// </summary>
        public void Destroy()
        {
            if(MarkedToBeRemoved)
                return;

            Service.DestroyEntity(this);

			// set servive reference as null for avoiding memory leak
			Service = null;
        }

        public override string ToString()
        {
            return $"{Id}: {Name}";
        }

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
