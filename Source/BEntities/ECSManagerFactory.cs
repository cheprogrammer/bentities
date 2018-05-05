using System;
using System.Collections.Generic;
using System.Reflection;

namespace BEntities
{
	/// <summary>
	/// Factory for building Entity Component System Manager
	/// </summary>
	public static class ECSManagerFactory
	{
		private static bool _assembliesScanned = false;

		private static readonly List<Type> AvailableSystems = new List<Type>();

        private static  readonly List<Type> AvailableTemplates = new List<Type>();

		/// <summary>
		/// Performs initial scan of assemblies for EC Systems
		/// </summary>
		/// <param name="assemblies"></param>
		public static void ScanAssemblies(params Assembly[] assemblies)
		{
			AvailableSystems.Clear();

			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();

				foreach (Type type in types)
				{
					// checking types for registering systems
					if (typeof(BaseComponentSystem).IsAssignableFrom(type) &&
						type.GetCustomAttribute<ComponentSystemAttribute>() != null)
					{
						AvailableSystems.Add(type);
					}

				    if (typeof(EntityTemplate).IsAssignableFrom(type) &&
				        type.GetCustomAttribute<EntityTemplateAttribute>() != null)
				    {
                        AvailableTemplates.Add(type);
				    }
				}
			}

			_assembliesScanned = true;
		}

		/// <summary>
		/// Creates fully initialized ECS Manager
		/// </summary>
		/// <returns></returns>
		public static ECSManager CreateECSManager()
		{
			if (!_assembliesScanned)
				throw new Exception($"Unable to create ECS Manager. Call method '{nameof(ScanAssemblies)}' before creating ECS Manager");

			ECSManager result = new ECSManager();

			// initialize ecs manager with available scanned systems
			result.Initialize(AvailableSystems, AvailableTemplates);

			return result;
		}
	}
}
