using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace BEntities
{
	/// <summary>
	/// Entity Component System manager class which is resposible for Entity creation, Systems registering and processing
	/// Incapsulates all interaction with internal ECS Service
	/// </summary>
    public class ECSManager
    {
        private readonly ECSService _service = new ECSService();
        
		internal ECSManager()
		{

		}
		
		/// <summary>
        /// Tetrieves the system by its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSystem<T>() where T : BaseComponentProcessingSystem
        {
            var result = _service.UpdateSystems.FirstOrDefault(e => e.GetType() == typeof(T));
            if (result != null)
                return (T)result;

            result = _service.DrawSystems.FirstOrDefault(e => e.GetType() == typeof(T));
            return (T)result;
        }

        /// <summary>
        /// Registers system by its type and performs all necessary initialization routine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterSystem<T>() where T : BaseComponentProcessingSystem
        {
            RegisterSystem(typeof(T));
        }

        /// <summary>
        /// Registers system by its type and performs all necessary initialization routine
        /// </summary>
        public void RegisterSystem(Type type)
        {
            if(!typeof(BaseComponentSystem).IsAssignableFrom(type))
                throw new ArgumentException($"EC System of type {type.Name} is not inherited from {nameof(BaseComponentSystem)}");

			BaseComponentSystem system = (BaseComponentSystem)Activator.CreateInstance(type);
            system.PreInitialize();

            if (system.SystemType == SystemProcessingType.Draw)
            {
                _service.DrawSystems.Add(system);
                _service.DrawSystems.Sort((system1, system2) => system1.Order - system2.Order);
                
            }
            else if (system.SystemType == SystemProcessingType.Update)
            {
                _service.UpdateSystems.Add(system);
                _service.UpdateSystems.Sort((system1, system2) => system1.Order - system2.Order);
            }

            system.Initialize();
        }

        /// <summary>
        /// Performs initialization of all scanned systems
        /// </summary>
        internal void Initialize(IEnumerable<Type> availableSystems)
        {
            foreach (Type availableSystem in availableSystems)
            {
                BaseComponentProcessingSystem system = (BaseComponentProcessingSystem)Activator.CreateInstance(availableSystem);
                system.PreInitialize();

                if (system.SystemType == SystemProcessingType.Draw)
                    _service.DrawSystems.Add(system);
                else if (system.SystemType == SystemProcessingType.Update)
                    _service.UpdateSystems.Add(system);
            }

            // performing ordering of systems and initializing
            _service.UpdateSystems.Sort((system1, system2) => system1.Order - system2.Order);
            _service.DrawSystems.Sort((system1, system2) => system1.Order - system2.Order);

            foreach (BaseComponentProcessingSystem system in _service.UpdateSystems)
            {
                system.Initialize();
            }

            foreach (BaseComponentProcessingSystem system in _service.DrawSystems)
            {
                system.Initialize();
            }
        }

		/// <summary>
		/// Performs the step procedure for Drawning systems
		/// </summary>
		/// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            foreach (BaseComponentProcessingSystem baseComponentSystem in _service.DrawSystems)
            {
                baseComponentSystem.Step(gameTime);
            }
        }

		/// <summary>
		/// Performs step procedure for Updating systems
		/// </summary>
		/// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _service.ProcessComponentsForRemoving();
            _service.ProcessEntitiesForRemoving();

            foreach (BaseComponentProcessingSystem serviceUpdateSystem in _service.UpdateSystems)
            {
                serviceUpdateSystem.Step(gameTime);
            }

            _service.ProcessEntitiesForAdding();
            _service.ProcessComponentsForRegistering();
        }

		/// <summary>
		/// Performs the immediate destruction of all registered entities
		/// </summary>
        public void DestroyAllEntities()
        {
            _service.DestroyAllEntitiesImmediately();
        }

		// Performs 
        public void UnloadContent()
        {
            DestroyAllEntities();
        }

        public Entity CreateEntity()
        {
            return _service.CreateEntity();
        }
    }
}
