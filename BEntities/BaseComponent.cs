using BEntities.Components;

namespace BEntities
{
    public abstract class BaseComponent
    {
        /// <summary>
        /// Gets the source entity
        /// </summary>
        internal Entity Entity { get; set; }

        public Transform2DComponent Transform => Entity.Transform;

        public bool MarkedToBeRemoved { get; internal set; } = false;

        public BaseComponent()
        {
            
        }

        public virtual void Reset()
        {
            
        }

        public T GetComponent<T>() where T: BaseComponent
        {
            return Entity.GetComponent<T>();
        }
    }
}
