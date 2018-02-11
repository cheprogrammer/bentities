using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BEntities
{
    public class EntityComponentSystemManager
    {
        private readonly ECSService _service = new ECSService();

        private static readonly List<Type> AvailableSystems = new List<Type>();

        /// <summary>
        /// Performs initial scan of assemblies for EC Systems
        /// </summary>
        /// <param name="assemblies"></param>
        public static void ScanAssemblies(params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetExportedTypes();

                foreach (Type type in types)
                {
                    // checking types for registering systems
                    if (typeof(BaseComponentSystem).IsAssignableFrom(type) &&
                        type.GetCustomAttribute<ComponentSystemAttribute>() != null)
                    {
                        AvailableSystems.Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// Tetrieves the system by its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSystem<T>() where T : BaseComponentSystem
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
        public void RegisterSystem<T>() where T : BaseComponentSystem
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
        public void Initialize()
        {
            foreach (Type availableSystem in AvailableSystems)
            {
                BaseComponentSystem system = (BaseComponentSystem)Activator.CreateInstance(availableSystem);
                system.PreInitialize();

                if (system.SystemType == SystemProcessingType.Draw)
                    _service.DrawSystems.Add(system);
                else if (system.SystemType == SystemProcessingType.Update)
                    _service.UpdateSystems.Add(system);
            }

            // performing ordering of systems and initializing
            _service.UpdateSystems.Sort((system1, system2) => system1.Order - system2.Order);
            _service.DrawSystems.Sort((system1, system2) => system1.Order - system2.Order);

            foreach (BaseComponentSystem system in _service.UpdateSystems)
            {
                system.Initialize();
            }

            foreach (BaseComponentSystem system in _service.DrawSystems)
            {
                system.Initialize();
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (BaseComponentSystem baseComponentSystem in _service.DrawSystems)
            {
                baseComponentSystem.Process(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            _service.ProcessComponentsForRemoving();
            _service.ProcessEntitiesForRemoving();

            foreach (BaseComponentSystem serviceUpdateSystem in _service.UpdateSystems)
            {
                serviceUpdateSystem.Process(gameTime);
            }

            _service.ProcessEntitiesForAdding();
            _service.ProcessComponentsForRegistering();
        }

        public void UnloadContent()
        {
            _service.DestroyAllEntitiesImmediately();
        }

        public Entity CreateEntity()
        {
            return _service.CreateEntity();
        }
    }
}
