using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BEntities
{
	public abstract class BaseComponentSystem
	{
		internal Type[] ComponentTypes { get; private set; }

		internal int Order { get; private set; }

		internal SystemProcessingType SystemType { get; private set; }

		internal List<BaseComponent> RegisteredComponents { get; } = new List<BaseComponent>();

        public ECSManager ECSManager { get; private set; }

		#region Internal accessibility methods 

		internal void PreInitialize(ECSManager sourceECSManager)
		{
			ComponentSystemAttribute systemAttribute = (ComponentSystemAttribute)this.GetType().GetCustomAttribute(typeof(ComponentSystemAttribute));

			if (systemAttribute == null)
				throw new Exception($"Component System of type {GetType().Name} does not have attribute {nameof(ComponentSystemAttribute)}");

			SystemType = systemAttribute.SystemProcessingType;
			Order = systemAttribute.Order;
			ComponentTypes = systemAttribute.Components;

		    ECSManager = sourceECSManager;
		}

		internal bool IsAppliableComponent(BaseComponent component)
		{
			return ComponentTypes.Any(e => e == component.GetType() || e.IsInstanceOfType(component));
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
			ComponentRegistered(component.SourceEntity, component);
			OnComponentRegistered?.Invoke(component.SourceEntity, component);
		}

		internal void OnComponentRemovedInternal(BaseComponent component)
		{
			ComponentRemoved(component.SourceEntity, component);
			OnComponentRemoved?.Invoke(component.SourceEntity, component);
		}

		#endregion


		#region Public

		public virtual void Initialize()
		{

		}

		public void Step(GameTime gameTime)
		{
			Process(gameTime, RegisteredComponents);
		}

		public event ComponentEventHandler OnComponentRegistered;

		public event ComponentEventHandler OnComponentRemoved;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Method performs processing of all available components.
		/// Note: In case of overriding this method, realization should take all resposibility for detecting Marked-to-be-removed components and Inactive entities
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="components"></param>
		protected virtual void Process(GameTime gameTime, IEnumerable<BaseComponent> components)
		{

		}

		protected virtual void ComponentRegistered(Entity entity, BaseComponent component)
		{

		}

		protected virtual void ComponentRemoved(Entity entity, BaseComponent component)
		{

		}

		#endregion
	}
}
