using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace BEntities
{
	/// <summary>
	/// Base abstract class for EC Processing Systems.
	/// The main difference between BaseComponentSystem - incapsulated Processing of all registered entities
	/// </summary>
    public abstract class BaseComponentProcessingSystem : BaseComponentSystem
    {
        #region Public

		/// <summary>
		/// Incapsulates all processing routine with poiniting of marked-to-be-removed components and inactive entities
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="components"></param>
        protected override void Process(GameTime gameTime, IEnumerable<BaseComponent> components)
        {
            foreach (BaseComponent registeredComponent in RegisteredComponents)
            {
				if(registeredComponent.SourceEntity.IsActive && !registeredComponent.MarkedToBeRemoved)
					ProcessComponent(gameTime, registeredComponent);
            }
        }

		/// <summary>
		/// Method for performing processing of one component
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="component"></param>
        protected virtual void ProcessComponent(GameTime gameTime, BaseComponent component)
        {

        }

        #endregion
    }
}
