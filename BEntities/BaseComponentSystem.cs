using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BEntities
{
    public abstract class BaseComponentSystem
    {
        internal Type[] ComponentTypes { get; private set; }

        internal int Order { get; private set; }

        internal SystemProcessingType SystemType { get; private set; }

        internal List<BaseComponent> RegisteredComponents { get; } = new List<BaseComponent>();

        #region Internal accessibility methods 

        internal void PreInitialize()
        {
            ComponentSystemAttribute systemAttribute = (ComponentSystemAttribute)this.GetType().GetCustomAttribute(typeof(ComponentSystemAttribute));

            if (systemAttribute == null)
                throw new Exception($"Component System of type {GetType().Name} does not have attribute {nameof(ComponentSystemAttribute)}");

            SystemType = systemAttribute.SystemProcessingType;
            Order = systemAttribute.Order;
            ComponentTypes = systemAttribute.Components;
        }

        internal bool IsAppliableComponent(BaseComponent component)
        {
            return ComponentTypes.Any(e => e == component.GetType());
        }

        internal void RegisterComponent(BaseComponent component)
        {
            RegisteredComponents.Add(component);
            OnComponentRegisteredInternal(component);
        }

        internal void UnregisterComponent(BaseComponent component)
        {
            RegisteredComponents.Remove(component);
            OnComponentRemovedInternal(component);
        }

        internal void OnComponentRegisteredInternal(BaseComponent component)
        {
            ComponentRegistered(component);
            OnComponentRegistered?.Invoke(component);
        }

        internal void OnComponentRemovedInternal(BaseComponent component)
        {
            ComponentRemoved(component);
            OnComponentRemoved?.Invoke(component);
        }

        #endregion


        #region Public

        public virtual void Initialize()
        {

        }

        public virtual void Process(GameTime gameTime)
        {
            foreach (BaseComponent registeredComponent in RegisteredComponents)
            {
                if (registeredComponent.Entity.IsActive && !registeredComponent.MarkedToBeRemoved)
                    ProcessComponent(gameTime, registeredComponent);
            }
        }

        public virtual void ProcessComponent(GameTime gameTime, BaseComponent component)
        {

        }

        public event ComponentEventHandler OnComponentRegistered;

        protected virtual void ComponentRegistered(BaseComponent component)
        {

        }

        public event ComponentEventHandler OnComponentRemoved;

        protected virtual void ComponentRemoved(BaseComponent component)
        {

        }

        #endregion

    }
}
