using BEntities.Components;

namespace BEntities
{
    public abstract class BaseComponent
    {
        /// <summary>
        /// Gets the source entity
        /// </summary>
        internal Entity SourceEntity { get; set; }

        public Transform2DComponent Transform => SourceEntity.Transform;

        public bool MarkedToBeRemoved { get; internal set; } = false;

        /// <summary>
        /// Performs initialization of Base component on registering it in systems
        /// </summary>
        public virtual void Initialize()
        {

        }

        public virtual void Reset()
        {
            
        }

        public T GetComponent<T>() where T: BaseComponent
        {
            return SourceEntity.GetComponent<T>();
        }

        public T GetComponentInParent<T>() where T : BaseComponent
        {
            return SourceEntity.GetComponentInParent<T>();
        }
    }
}
