using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEntities.Components;

namespace BEntities
{
    public class Entity
    {
        public int Id { get; internal set; }

        public string Name { get; set; } = "Entity";

        internal ECSService Service { get; set; }

        public Transform2DComponent Transform
        {
            get
            {
                BaseComponent result;

                if (AttachedComponents.TryGetValue(typeof(Transform2DComponent), out result))
                    return (Transform2DComponent)result;

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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T AttachComponent<T>() where T : BaseComponent, new()
        {
            return Service.AttachComponent<T>(this);
        }

        /// <summary>
        /// Detaches and releases component from current entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DetachComponent<T>() where T : BaseComponent
        {
            DetachComponent(typeof(T));
        }

        /// <summary>
        /// Detaches and releases component from current entity
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

        public T GetComponent<T>() where T : BaseComponent
        {
            BaseComponent result;

            if (AttachedComponents.TryGetValue(typeof(T), out result))
            {
                return (T) result;
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}
